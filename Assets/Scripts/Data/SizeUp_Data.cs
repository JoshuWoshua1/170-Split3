using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "SizeUp_Data", menuName = "ScriptableObjects/SizeUp_Data", order = 1)]
public class SizeUp_Data : ScriptableObject
{
    /* ------Resources & Their IDs-------
    // 1: Water
    // 2: Glucose
    // 3: Amino Acids
    // 4: Deoxyribose
    // 5: n/a
    // 6: n/a
    */
    public List<SizeTier> tiers = new List<SizeTier>();
}

[Serializable]
public class SizeTier
{
    public float targetSize;
    public List<ResourceRequirement> resourceThresholds = new List<ResourceRequirement>();
}

[Serializable]
public class ResourceRequirement
{
    public ResourceType resourceType;
    public int amount;
}