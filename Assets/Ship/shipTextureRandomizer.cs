using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipTextureRandomizer : MonoBehaviour
{
    [SerializeField] private List<Texture> albedoTextures = new List<Texture>();
    [SerializeField] private List<Color> emissiveColors = new List<Color>();
    [SerializeField] private List<Color> thrusterColors = new List<Color>();
    [SerializeField] private ParticleSystem thrusterRight, thrusterLeft;

    // Start is called before the first frame update
    void Start()
    {
        //pick and assign an albedo texture at random
        int textureIndex = Random.Range(0, albedoTextures.Count);
        this.GetComponent<Renderer>().material.mainTexture = albedoTextures[textureIndex];
        MaintainCoolant.SetMatEmission(this.GetComponent<Renderer>().material, true, emissiveColors[textureIndex], -1f);
        var tr = thrusterRight.main;
        tr.startColor = thrusterColors[textureIndex];
        var tl = thrusterLeft.main;
        tl.startColor = thrusterColors[textureIndex];
    }
}
