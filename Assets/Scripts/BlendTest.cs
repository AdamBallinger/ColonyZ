using UnityEngine;

public class BlendTest : MonoBehaviour
{
    public Sprite mainSprite;
    public Sprite sprite;
    private Renderer ren;

    private void Start()
    {
        ren = GetComponent<Renderer>();
        
        var block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", mainSprite.texture);
        block.SetTexture("_BlendTex", sprite.texture);
        ren.SetPropertyBlock(block);
    }
}