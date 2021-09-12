using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TreeHealthSystem : MonoBehaviour
{
    public static TreeHealthSystem Instance;

    public Image currentHealthBar;
    public Text healthText;
    private float hitPoint;
    private float maxHitPoint;

    private Tree_behavior tb;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tb = GameObject.Find("Tree").GetComponent<Tree_behavior>();
        hitPoint = tb.health;
        maxHitPoint = tb.maxhealth;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float ratio = hitPoint / maxHitPoint;
        currentHealthBar.rectTransform.localPosition = new Vector3(currentHealthBar.rectTransform.rect.width * ratio - currentHealthBar.rectTransform.rect.width, 0, 0);
        healthText.text = hitPoint.ToString("0") + "/" + maxHitPoint.ToString("0");
    }

    public void SetHealth(float Health)
    {
        if (Health <= 0)
            hitPoint = 0;
        else
            hitPoint = Health;
        UpdateHealthBar();
    }

    public void TakeDamage(float Damage)
    {
        hitPoint -= Damage;
        if (hitPoint < 1)
            hitPoint = 0;

        UpdateHealthBar();
    }

    public void HealDamage(float Heal)
    {
        hitPoint += Heal;
        if (hitPoint > maxHitPoint)
            hitPoint = maxHitPoint;

        UpdateHealthBar();
    }

    public void SetMaxHealth(float max)
    {
        maxHitPoint += (int)(maxHitPoint * max / 100);
        UpdateHealthBar();
    }
    
}
