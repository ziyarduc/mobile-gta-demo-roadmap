namespace GTAClone.Core
{
    /// <summary>
    /// Oyunun ve oyuncunun o anki temel durumunu belirten enum.
    /// </summary>
    public enum GameState
    {
        OnFoot,     // Karakter yaya modunda, serbest dolaşımda
        InVehicle,  // Karakter bir aracın içinde, araç sürüş modunda
        Paused,     // Oyun duraklatılmış (Pause menüsü açık)
        Menu        // Ana menü veya envanter/harita ekranı açık
    }
}
