using UnityEngine;
using System.Collections;

public class ParticleFader : MonoBehaviour
{
    [Tooltip("淡出持续时间(秒)")]
    public float fadeDuration = 5.0f;
    
    [Tooltip("淡出完成后是否自动销毁对象")]
    public bool destroyOnComplete = true;
    
    private ParticleSystem particleSystem;
    public ParticleSystem.Particle[] particles;
    private float[] initialAlphas; // 存储每个粒子的初始透明度
    private bool isFading = false;
    private float fadeStartTime;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("未找到粒子系统组件!");
            return;
        }
        
        // 初始化粒子数组
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        initialAlphas = new float[particleSystem.main.maxParticles];
         StartFadeOut();
    }

    // 开始淡出效果
    public void StartFadeOut()
    {
        if (isFading) return;
        
        // 获取当前所有粒子并记录它们的初始透明度
        int numParticles = particleSystem.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {
            initialAlphas[i] = particles[i].GetCurrentColor(particleSystem).a;
        }
        
        isFading = true;
        fadeStartTime = Time.time;
        
        // 停止发射新粒子
        particleSystem.Stop();
        
        StartCoroutine(FadeOutCoroutine());
    }

    // 淡出协程
    private IEnumerator FadeOutCoroutine()
    {
        while (isFading)
        {
            float elapsedTime = Time.time - fadeStartTime;
            float fadeProgress = Mathf.Clamp01(elapsedTime / fadeDuration);
            
            // 获取当前粒子
            int numParticles = particleSystem.GetParticles(particles);
            
            for (int i = 0; i < numParticles; i++)
            {
                // 计算当前透明度 (从初始值渐变到0)
                float targetAlpha = Mathf.Lerp(initialAlphas[i], 0f, fadeProgress);
                
                // 获取粒子当前颜色并修改透明度
                Color particleColor = particles[i].GetCurrentColor(particleSystem);
                particleColor.a = targetAlpha;
                particles[i].startColor = particleColor;
            }
            
            // 应用修改后的粒子数据
            particleSystem.SetParticles(particles, numParticles);
            
            // 检查是否完成淡出
            if (fadeProgress >= 1.0f)
            {
                isFading = false;
                
                if (destroyOnComplete)
                {
                    Destroy(gameObject);
                }
                else
                {
                    particleSystem.Clear(); // 清除所有粒子
                    enabled = false; // 禁用脚本
                }
                
                yield break;
            }
            
            yield return null;
        }
    }

   

    // 公共方法：可以从其他脚本调用开始淡出
    public void BeginFadeOut(float duration = 5f)
    {
        fadeDuration = duration;
        StartFadeOut();
    }
}