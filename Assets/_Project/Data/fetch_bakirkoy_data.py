import urllib.request
import json
import math
import os

# Bakırköy Merkez (Özgürlük Meydanı, Ebuzziya Caddesi, Marmaray, Sahil Parkı)
# Bounding Box: min_lat, min_lon, max_lat, max_lon
MIN_LAT = 40.9720
MIN_LON = 28.8680
MAX_LAT = 40.9810
MAX_LON = 28.8820

CENTER_LAT = (MIN_LAT + MAX_LAT) / 2.0
CENTER_LON = (MIN_LON + MAX_LON) / 2.0

# Mercator / Flat Earth Projection metreye dönüştürme
METERS_PER_DEG_LAT = 111132.954 - 559.822 * math.cos(2 * math.radians(CENTER_LAT))
METERS_PER_DEG_LON = 111412.84 * math.cos(math.radians(CENTER_LAT))

def lat_lon_to_meters(lat, lon):
    x = (lon - CENTER_LON) * METERS_PER_DEG_LON
    z = (lat - CENTER_LAT) * METERS_PER_DEG_LAT
    return round(x, 2), round(z, 2)

overpass_query = f"""
[out:json][timeout:30];
(
  // Binalar
  way["building"]({MIN_LAT},{MIN_LON},{MAX_LAT},{MAX_LON});
  // Yollar
  way["highway"]({MIN_LAT},{MIN_LON},{MAX_LAT},{MAX_LON});
  // Parklar ve yeşil alanlar
  way["leisure"="park"]({MIN_LAT},{MIN_LON},{MAX_LAT},{MAX_LON});
);
out body;
>;
out skel qt;
"""

url = "https://overpass-api.de/api/interpreter"

print("Overpass API'den Bakırköy verileri çekiliyor...")
req = urllib.request.Request(
    url,
    data=overpass_query.encode('utf-8'),
    headers={'User-Agent': 'BakirkoyGTACloneGenerator/1.0'}
)

try:
    with urllib.request.urlopen(req, timeout=35) as response:
        osm_data = json.loads(response.read().decode('utf-8'))
        
    elements = osm_data.get('elements', [])
    print(f"Toplam {len(elements)} element alındı.")

    nodes = {}
    ways = []
    
    for el in elements:
        if el['type'] == 'node':
            nodes[el['id']] = (el['lat'], el['lon'])
        elif el['type'] == 'way':
            ways.append(el)

    buildings = []
    roads = []
    parks = []

    for way in ways:
        tags = way.get('tags', {})
        node_ids = way.get('nodes', [])
        points = []
        for nid in node_ids:
            if nid in nodes:
                lat, lon = nodes[nid]
                x, z = lat_lon_to_meters(lat, lon)
                points.append({"x": x, "z": z})
        
        if len(points) < 2:
            continue

        name = tags.get('name', tags.get('name:tr', ''))

        if 'building' in tags:
            # Kat sayısı veya yükseklik
            levels = tags.get('building:levels', '4')
            try:
                height = float(tags.get('height', float(levels) * 3.2))
            except:
                height = 12.0
            
            building_type = tags.get('building', 'yes')
            buildings.append({
                "name": name if name else "Bina",
                "type": building_type,
                "height": round(height, 1),
                "points": points
            })
        elif 'highway' in tags:
            highway_type = tags.get('highway', 'residential')
            lanes = 2
            if highway_type in ['primary', 'secondary', 'trunk']:
                lanes = 4
            elif highway_type in ['pedestrian', 'footway', 'path']:
                lanes = 1

            roads.append({
                "name": name if name else "Cadde/Sokak",
                "type": highway_type,
                "lanes": lanes,
                "points": points
            })
        elif tags.get('leisure') == 'park':
            parks.append({
                "name": name if name else "Park/Yeşil Alan",
                "points": points
            })

    # Bakırköy İkonik Noktaları (Landmarks)
    landmarks = [
        {"name": "Bakırköy Özgürlük Meydanı", "type": "Zone_Central", "pos": {"x": 0, "z": 50}, "radius": 40},
        {"name": "Ebuzziya Caddesi (Çarşı)", "type": "Zone_Commercial", "pos": {"x": -20, "z": -120}, "radius": 50},
        {"name": "Bakırköy Marmaray İstasyonu", "type": "Zone_Transit", "pos": {"x": -10, "z": -30}, "radius": 30},
        {"name": "Bakırköy Sahil Parkı & Kennedy Cad.", "type": "Zone_Coast", "pos": {"x": 20, "z": -400}, "radius": 80},
        {"name": "Capacity & Carousel Bölgesi", "type": "Zone_Mall", "pos": {"x": -150, "z": 80}, "radius": 45}
    ]

    output_data = {
        "location": "Bakırköy, İstanbul",
        "center_lat": CENTER_LAT,
        "center_lon": CENTER_LON,
        "landmarks": landmarks,
        "total_buildings": len(buildings),
        "total_roads": len(roads),
        "total_parks": len(parks),
        "buildings": buildings,
        "roads": roads,
        "parks": parks
    }

    out_dir = r"C:\Users\silver\.gemini\antigravity\worktrees\vibrant-borg\mobile-gta-demo-roadmap\Assets\_Project\Data"
    os.makedirs(out_dir, exist_ok=True)
    out_file = os.path.join(out_dir, "bakirkoy_map_data.json")

    with open(out_file, "w", encoding="utf-8") as f:
        json.dump(output_data, f, ensure_ascii=False, indent=2)

    print(f"Başarılı! {len(buildings)} bina, {len(roads)} yol ve {len(parks)} park verisi kaydedildi: {out_file}")

except Exception as e:
    print(f"Hata oluştu: {e}")
