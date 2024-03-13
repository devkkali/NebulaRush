using UnityEngine;

public class BGScroll : MonoBehaviour
{
    public float speed;
    public Renderer bgRenderer;
    [SerializeField] private float scorePerUnit = 1f; // Points per unit of distance


    private void Update()
    {
        var distanceThisFrame = speed * Time.deltaTime;
        bgRenderer.material.mainTextureOffset += new Vector2(distanceThisFrame, 0);

        // Call the method to add score based on the distance traveled
        ScoreManager.Instance.AddDistanceScore(distanceThisFrame, scorePerUnit);
    }
}