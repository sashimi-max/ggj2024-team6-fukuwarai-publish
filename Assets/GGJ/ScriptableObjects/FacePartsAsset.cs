using GGJ.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FacePartsData", menuName = "ScriptableObjects/CreateFacePartsSet")]
public class FacePartsAsset : ScriptableObject
{
    public List<FaceParts> facePartsSet = new List<FaceParts>();
}

[System.Serializable]
public class FaceParts
{
    public string name = "人間セット";
    public Sprite wholeFaceSprite;
    public FacePartsData[] downEjectedFacePartsData;
    public FacePartsData[] rightEjectedFacePartsData;
    public FacePartsData[] upEjectedFacePartsData;
    public FacePartsData[] leftEjectedFacePartsData;
}

[System.Serializable]
public class FacePartsData
{
    public string name;
    public Sprite sprite;
    public CollidableObjectType collidableObjectType;
    public float drag = 1.0f;
}
