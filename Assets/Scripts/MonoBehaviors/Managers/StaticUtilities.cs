using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticUtilities
{
    public static readonly int placableLayer = 1<< LayerMask.NameToLayer("Placable");
    public static readonly int nonPlacableLayer = 1<< LayerMask.NameToLayer("NotPlacable");
    public static readonly int PlacingLayers = placableLayer | nonPlacableLayer;
    public static readonly Dictionary<TurretType, Color> colorConversion = new Dictionary<TurretType, Color>
    {
        {TurretType.Explosive, Color.red},
        { TurretType.Laser, Color.cyan}
    };
}
