using System;
using System.Collections.Generic;
using UnityEngine;

namespace GTAClone.World
{
    [System.Serializable]
    public class MapPoint
    {
        public float x;
        public float z;
    }

    [System.Serializable]
    public class BuildingData
    {
        public string name;
        public string type;
        public float height;
        public List<MapPoint> points;
    }

    [System.Serializable]
    public class RoadData
    {
        public string name;
        public string type;
        public int lanes;
        public List<MapPoint> points;
    }

    [System.Serializable]
    public class LandmarkData
    {
        public string name;
        public string type;
        public MapPoint pos;
        public float radius;
    }

    [System.Serializable]
    public class BakirkoyMapDataset
    {
        public string location;
        public List<LandmarkData> landmarks;
        public List<BuildingData> buildings;
        public List<RoadData> roads;
    }

    /// <summary>
    /// Gerçek dünya Bakırköy OpenStreetMap verilerini Unity'de 3D bina ve yollara dönüştüren Prosedürel Şehir Oluşturucu.
    /// </summary>
    public class BakirkoyWorldBuilder : MonoBehaviour
    {
        [Header("Data Source")]
        [SerializeField] private TextAsset mapJsonFile;

        [Header("Materials (Mobil URP)")]
        [SerializeField] private Material buildingMaterial;
        [SerializeField] private Material roadMaterial;
        [SerializeField] private Material parkMaterial;

        [Header("Optimization Settings")]
        [SerializeField] private bool generateColliders = true;
        [SerializeField] private float maxBuildingDistance = 800f; // Merkezden max mesafe (mobil performans için)

        [ContextMenu("Şehri Oluştur (Generate Bakırköy City)")]
        public void GenerateCity()
        {
            if (mapJsonFile == null)
            {
                Debug.LogError("[BakirkoyWorldBuilder] Lütfen 'bakirkoy_map_data.json' dosyasını 'Map Json File' alanına sürükleyin!");
                return;
            }

            ClearExistingCity();

            BakirkoyMapDataset dataset = JsonUtility.FromJson<BakirkoyMapDataset>(mapJsonFile.text);
            if (dataset == null)
            {
                Debug.LogError("[BakirkoyWorldBuilder] JSON verisi ayrıştırılamadı!");
                return;
            }

            Debug.Log($"[BakirkoyWorldBuilder] {dataset.location} haritası inşa ediliyor... Toplam Bina: {dataset.buildings?.Count}, Yol: {dataset.roads?.Count}");

            Transform buildingsParent = new GameObject("--- Binalar (Buildings) ---").transform;
            buildingsParent.SetParent(transform);

            Transform roadsParent = new GameObject("--- Yollar (Roads) ---").transform;
            roadsParent.SetParent(transform);

            Transform landmarksParent = new GameObject("--- İkonik Bölgeler (Landmarks) ---").transform;
            landmarksParent.SetParent(transform);

            // 1. Binaları İnşa Et
            if (dataset.buildings != null)
            {
                int count = 0;
                foreach (var b in dataset.buildings)
                {
                    if (b.points == null || b.points.Count < 3) continue;

                    Vector3 center = CalculateCenter(b.points);
                    if (center.magnitude > maxBuildingDistance) continue; // Mesafe optimizasyonu

                    GameObject bObj = CreateExtrudedBuilding(b, center, buildingsParent);
                    count++;
                }
                Debug.Log($"[BakirkoyWorldBuilder] {count} adet Bakırköy binası 3D olarak oluşturuldu.");
            }

            // 2. Yolları Çiz
            if (dataset.roads != null)
            {
                foreach (var r in dataset.roads)
                {
                    if (r.points == null || r.points.Count < 2) continue;
                    CreateRoadSegment(r, roadsParent);
                }
            }

            // 3. İkonik Noktaları ve GTA Bölgelerini Yerleştir
            if (dataset.landmarks != null)
            {
                foreach (var lm in dataset.landmarks)
                {
                    CreateLandmarkMarker(lm, landmarksParent);
                }
            }

            Debug.Log("[BakirkoyWorldBuilder] ✅ Bakırköy Demo Haritası Başarıyla İnşa Edildi!");
        }

        [ContextMenu("Şehri Temizle (Clear)")]
        public void ClearExistingCity()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private GameObject CreateExtrudedBuilding(BuildingData b, Vector3 center, Transform parent)
        {
            GameObject bObj = new GameObject($"Bina_{b.name}");
            bObj.transform.position = new Vector3(center.x, 0, center.z);
            bObj.transform.SetParent(parent);

            MeshFilter mf = bObj.AddComponent<MeshFilter>();
            MeshRenderer mr = bObj.AddComponent<MeshRenderer>();
            mr.material = buildingMaterial != null ? buildingMaterial : GetDefaultMaterial(new Color(0.8f, 0.78f, 0.75f));

            Mesh mesh = new Mesh();
            mesh.name = "BuildingMesh";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int count = b.points.Count;
            float h = Mathf.Clamp(b.height, 4f, 40f);

            // Yan Duvarlar (Walls)
            for (int i = 0; i < count; i++)
            {
                int next = (i + 1) % count;
                Vector3 p1 = new Vector3(b.points[i].x - center.x, 0, b.points[i].z - center.z);
                Vector3 p2 = new Vector3(b.points[next].x - center.x, 0, b.points[next].z - center.z);
                Vector3 p1_top = new Vector3(p1.x, h, p1.z);
                Vector3 p2_top = new Vector3(p2.x, h, p2.z);

                int startIndex = vertices.Count;

                vertices.Add(p1);
                vertices.Add(p2);
                vertices.Add(p1_top);
                vertices.Add(p2_top);

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));

                triangles.Add(startIndex);
                triangles.Add(startIndex + 2);
                triangles.Add(startIndex + 1);

                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);
                triangles.Add(startIndex + 3);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mf.sharedMesh = mesh;

            if (generateColliders)
            {
                MeshCollider mc = bObj.AddComponent<MeshCollider>();
                mc.sharedMesh = mesh;
            }

            return bObj;
        }

        private void CreateRoadSegment(RoadData r, Transform parent)
        {
            GameObject roadObj = new GameObject($"Yol_{r.name}");
            roadObj.transform.SetParent(parent);

            LineRenderer lr = roadObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.positionCount = r.points.Count;
            lr.startWidth = r.lanes * 3.5f;
            lr.endWidth = r.lanes * 3.5f;

            for (int i = 0; i < r.points.Count; i++)
            {
                lr.SetPosition(i, new Vector3(r.points[i].x, 0.05f, r.points[i].z));
            }

            lr.material = roadMaterial != null ? roadMaterial : GetDefaultMaterial(new Color(0.2f, 0.2f, 0.2f));
        }

        private void CreateLandmarkMarker(LandmarkData lm, Transform parent)
        {
            GameObject lmObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lmObj.name = $"[Bölge] {lm.name}";
            lmObj.transform.position = new Vector3(lm.pos.x, 0.5f, lm.pos.z);
            lmObj.transform.localScale = new Vector3(lm.radius * 0.5f, 0.2f, lm.radius * 0.5f);
            lmObj.transform.SetParent(parent);

            Renderer ren = lmObj.GetComponent<Renderer>();
            ren.material = GetDefaultMaterial(new Color(1f, 0.8f, 0f, 0.4f));

            // GTA Bölge Satın Alma Trigger bileşeni
            var zoneTrigger = lmObj.AddComponent<GTAClone.Zone.ZoneTrigger>();
        }

        private Vector3 CalculateCenter(List<MapPoint> points)
        {
            float totalX = 0;
            float totalZ = 0;
            foreach (var p in points)
            {
                totalX += p.x;
                totalZ += p.z;
            }
            return new Vector3(totalX / points.Count, 0, totalZ / points.Count);
        }

        private Material GetDefaultMaterial(Color color)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            mat.color = color;
            return mat;
        }
    }
}
