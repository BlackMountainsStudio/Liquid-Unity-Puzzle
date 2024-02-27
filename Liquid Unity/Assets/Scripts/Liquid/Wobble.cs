using UnityEngine;

public class Wobble : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] private float maxWobble = 0.03f;
    [SerializeField] private float wobbleSpeed = 1f;
    [SerializeField] private float recovery = 1f;
    Renderer rend;
    Vector3 velocity;
    Vector3 angularVelocity;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    private static readonly int WobbleZ = Shader.PropertyToID("_WobbleZ");
    private static readonly int WobbleX = Shader.PropertyToID("_WobbleX");

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    
    private void Update()
    {
        time += Time.deltaTime;
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (recovery));

        pulse = 2 * Mathf.PI * wobbleSpeed*Time.deltaTime;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        rend.material.SetFloat(WobbleX, wobbleAmountX);
        rend.material.SetFloat(WobbleZ, wobbleAmountZ);

        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;

        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * maxWobble, -maxWobble, maxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * maxWobble, -maxWobble, maxWobble);

    }


}