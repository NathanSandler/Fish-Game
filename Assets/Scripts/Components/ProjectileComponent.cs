using Unity.Entities;


public struct ProjectileComponentBlob : IComponentData
{
    public BlobAssetReference<ProjectileComponentConfig> Blob;
    public Entity OnHit;
}

public struct ProjectileComponentConfig : IComponentData
{
    //public Entity OnHit;
    public int MaxImpactCount;
    public float Damage;
    public float Speed;
}

