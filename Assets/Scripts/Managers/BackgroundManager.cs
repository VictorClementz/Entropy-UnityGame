using UnityEngine;
using System.Collections.Generic;

public class SciFiSpaceBackground : MonoBehaviour
{
    [Header("=== BACKGROUND GRADIENT ===")]
    [SerializeField] private Color topColor = new Color(0.01f, 0.02f, 0.05f, 1f);
    [SerializeField] private Color bottomColor = new Color(0.02f, 0.05f, 0.12f, 1f);
    [SerializeField] private float gradientHeight = 200f;

    [Header("=== STARFIELD ===")]
    [SerializeField] private int starCount = 200;
    [SerializeField] private float fieldRadius = 100f;
    [SerializeField] private float fieldDepth = 50f;
    [SerializeField] private AnimationCurve starSizeDistribution = AnimationCurve.Linear(0, 0.02f, 1, 0.15f);
    [SerializeField] private float starBrightness = 2f;

    [Header("=== STAR COLORS ===")]
    [SerializeField]
    private Color[] starColors = new Color[] {
        new Color(1f, 1f, 1f, 1f),        // Pure white
        new Color(0.9f, 0.95f, 1f, 1f),   // Cool white
        new Color(0.8f, 0.9f, 1f, 1f),    // Blue-white
        new Color(1f, 0.95f, 0.9f, 1f)    // Warm white
    };

    [Header("=== DUST CLOUDS ===")]
    [SerializeField] private bool useDustClouds = true;
    [SerializeField] private int dustCloudCount = 3;
    [SerializeField] private Color dustColor = new Color(0.05f, 0.15f, 0.25f, 0.03f);
    [SerializeField] private float dustSize = 40f;

    [Header("=== ANIMATION ===")]
    [SerializeField] private float driftSpeed = 0.5f;
    [SerializeField] private float twinkleSpeed = 2f;
    [SerializeField] private float twinkleAmount = 0.3f;

    [Header("=== CAMERA ===")]
    [SerializeField] private Camera targetCamera;

    private GameObject starContainer;
    private GameObject backgroundPlane;
    private GameObject dustContainer;
    private List<StarData> stars = new List<StarData>();
    private Material starMaterial;
    private Material backgroundMaterial;
    private Material dustMaterial;

    private class StarData
    {
        public Transform transform;
        public Renderer renderer;
        public float twinkleOffset;
        public float twinkleSpeed;
        public Color originalColor;
        public float originalBrightness;
    }

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        CreateMaterials();
        CreateBackground();
        CreateStarfield();
        if (useDustClouds)
            CreateDustClouds();

        SetupCamera();
    }

    void CreateMaterials()
    {
        // Create star material with unlit shader for consistent appearance
        Shader unlitShader = Shader.Find("Unlit/Color");
        if (unlitShader == null)
            unlitShader = Shader.Find("Sprites/Default");

        starMaterial = new Material(unlitShader);

        // Create background gradient material
        Shader standardShader = Shader.Find("Standard");
        if (standardShader != null)
        {
            backgroundMaterial = new Material(standardShader);
            backgroundMaterial.color = bottomColor;
        }

        // Create dust material
        Shader transparentShader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        if (transparentShader == null)
            transparentShader = Shader.Find("Standard");

        dustMaterial = new Material(transparentShader);
        if (transparentShader.name.Contains("Standard"))
        {
            dustMaterial.SetFloat("_Mode", 3);
            dustMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            dustMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            dustMaterial.SetInt("_ZWrite", 0);
            dustMaterial.DisableKeyword("_ALPHATEST_ON");
            dustMaterial.EnableKeyword("_ALPHABLEND_ON");
            dustMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            dustMaterial.renderQueue = 3000;
        }
        dustMaterial.color = dustColor;
    }

    void CreateBackground()
    {
        // Create gradient background plane
        backgroundPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        backgroundPlane.name = "Background Gradient";
        backgroundPlane.transform.position = new Vector3(0, 0, -fieldDepth - 10);
        backgroundPlane.transform.localScale = new Vector3(fieldRadius * 4, gradientHeight, 1);

        Destroy(backgroundPlane.GetComponent<Collider>());

        // Create gradient mesh
        MeshFilter meshFilter = backgroundPlane.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Color[] colors = new Color[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            float heightPercent = (mesh.vertices[i].y + 0.5f);
            colors[i] = Color.Lerp(bottomColor, topColor, heightPercent);
        }
        mesh.colors = colors;

        // Apply material
        Renderer bgRenderer = backgroundPlane.GetComponent<Renderer>();
        bgRenderer.material = backgroundMaterial;
        bgRenderer.material.SetFloat("_Metallic", 0);
        bgRenderer.material.SetFloat("_Smoothness", 0);
    }

    void CreateStarfield()
    {
        starContainer = new GameObject("Starfield");

        for (int i = 0; i < starCount; i++)
        {
            CreateStar(i);
        }
    }

    void CreateStar(int index)
    {
        // Create star quad (more efficient than sphere)
        GameObject star = GameObject.CreatePrimitive(PrimitiveType.Quad);
        star.name = $"Star_{index}";
        star.transform.parent = starContainer.transform;

        // Remove collider
        Destroy(star.GetComponent<Collider>());

        // Random position in 3D space
        float x = Random.Range(-fieldRadius, fieldRadius);
        float y = Random.Range(-fieldRadius * 0.5f, fieldRadius * 0.5f);
        float z = Random.Range(-fieldDepth, -10f); // Behind gameplay area

        star.transform.position = new Vector3(x, y, z);

        // Make star face camera
        star.transform.LookAt(targetCamera.transform);
        star.transform.Rotate(0, 180, 0);

        // Random size based on distribution curve
        float sizeValue = Random.value;
        float size = starSizeDistribution.Evaluate(sizeValue);
        star.transform.localScale = Vector3.one * size;

        // Setup material and color
        Renderer renderer = star.GetComponent<Renderer>();
        Material instanceMat = new Material(starMaterial);

        // Pick random color from palette
        Color starColor = starColors[Random.Range(0, starColors.Length)];

        // Vary brightness
        float brightness = Random.Range(0.5f, 1f) * starBrightness;
        starColor *= brightness;
        starColor.a = 1f; // Ensure full opacity

        instanceMat.color = starColor;
        renderer.material = instanceMat;

        // Store star data for animation
        StarData data = new StarData
        {
            transform = star.transform,
            renderer = renderer,
            twinkleOffset = Random.Range(0f, Mathf.PI * 2f),
            twinkleSpeed = Random.Range(twinkleSpeed * 0.5f, twinkleSpeed * 1.5f),
            originalColor = starColor,
            originalBrightness = brightness
        };

        stars.Add(data);
    }

    void CreateDustClouds()
    {
        dustContainer = new GameObject("Dust Clouds");

        for (int i = 0; i < dustCloudCount; i++)
        {
            GameObject dust = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dust.name = $"Dust_{i}";
            dust.transform.parent = dustContainer.transform;

            Destroy(dust.GetComponent<Collider>());

            // Position dust clouds
            float x = Random.Range(-fieldRadius * 0.7f, fieldRadius * 0.7f);
            float y = Random.Range(-fieldRadius * 0.3f, fieldRadius * 0.3f);
            float z = Random.Range(-fieldDepth * 0.8f, -fieldDepth * 0.3f);

            dust.transform.position = new Vector3(x, y, z);

            // Scale
            float scale = Random.Range(dustSize * 0.7f, dustSize * 1.3f);
            dust.transform.localScale = Vector3.one * scale;

            // Apply translucent material
            Renderer renderer = dust.GetComponent<Renderer>();
            Material instanceMat = new Material(dustMaterial);

            // Vary the opacity slightly
            Color color = dustColor;
            color.a *= Random.Range(0.5f, 1f);
            instanceMat.color = color;

            renderer.material = instanceMat;
        }
    }

    void SetupCamera()
    {
        if (targetCamera != null)
        {
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
            targetCamera.backgroundColor = bottomColor;

            // Disable fog to keep it clean
            RenderSettings.fog = false;

            // Set ambient light to match
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.1f, 0.15f, 0.2f);
        }
    }

    void Update()
    {
        // Slow drift animation
        if (starContainer != null)
        {
            starContainer.transform.position += Vector3.right * driftSpeed * 0.1f * Time.deltaTime;

            // Wrap around
            if (starContainer.transform.position.x > 5f)
                starContainer.transform.position = new Vector3(-5f, 0, 0);
        }

        // Dust cloud drift (opposite direction for parallax)
        if (dustContainer != null)
        {
            dustContainer.transform.position += Vector3.left * driftSpeed * 0.05f * Time.deltaTime;

            if (dustContainer.transform.position.x < -3f)
                dustContainer.transform.position = new Vector3(3f, 0, 0);
        }

        // Twinkle animation
        float time = Time.time;
        foreach (StarData star in stars)
        {
            if (star.renderer != null && star.transform != null)
            {
                // Make stars face camera
                star.transform.LookAt(targetCamera.transform);
                star.transform.Rotate(0, 180, 0);

                // Twinkle effect
                float twinkle = Mathf.Sin(time * star.twinkleSpeed + star.twinkleOffset);
                twinkle = twinkle * twinkleAmount + (1f - twinkleAmount);

                Color newColor = star.originalColor * twinkle;
                newColor.a = 1f;
                star.renderer.material.color = newColor;
            }
        }
    }

    void OnValidate()
    {
        // Ensure we have at least some stars
        starCount = Mathf.Max(10, starCount);

        // Ensure star colors array isn't empty
        if (starColors == null || starColors.Length == 0)
        {
            starColors = new Color[] {
                Color.white,
                new Color(0.9f, 0.95f, 1f, 1f)
            };
        }

        // Validate animation curve
        if (starSizeDistribution == null || starSizeDistribution.keys.Length == 0)
        {
            starSizeDistribution = AnimationCurve.Linear(0, 0.02f, 1, 0.15f);
        }
    }
}