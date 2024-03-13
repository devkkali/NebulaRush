using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBossHealth : MonoBehaviour
{
	public Image healthFill; // The fillable part of the health bar
	public GameObject dividerPrefab; // The prefab for the divider
	public Transform dividerContainer; // The container to hold dividers
	public int maxHealth = 3; // Change maxLives to maxHealth
	public int currentHealth; // Change currentLives to currentHealth
	public Image lifeIcon; // The movable icon

	public static bool IsExploding { get; private set; }

	private void Awake()
	{
		currentHealth = maxHealth;
		StartCoroutine(CreateDividersAfterDelay());
	}

	private IEnumerator CreateDividersAfterDelay()
	{
		// Wait for the end of the frame to ensure all layout elements have been set up
		yield return new WaitForEndOfFrame();

		// Force a rebuild of the layout to get accurate positioning
		LayoutRebuilder.ForceRebuildLayoutImmediate(healthFill.rectTransform.parent.GetComponent<RectTransform>());

		CreateDividers();
		UpdateHealthBar();
	}

	public void TakeDamage(int damageAmount, GameObject boss)
	{
		if (IsExploding) return;
		currentHealth = Mathf.Max(currentHealth - damageAmount, 0); // Ensure health doesn't go below 0
		Debug.Log("current health is " + currentHealth);
		
		UpdateHealthBar();

		if (currentHealth <= 0)
		{
			// Destroy(boss);
			IsExploding = true;
			gameObject.SetActive(false);
			LevelManager.Instance.BossDefeated(boss);
			// Boss defeated, you can add further logic here like triggering an animation or ending the level
			// Debug.Log("Boss defeated!");
		}
	}
	// Call this when the boss is done exploding and ready to be cleaned up
	public static void ResetExplosion()
	{
		IsExploding = false;
	}
	private void UpdateHealthBar()
	{
		// Update the health fill amount
		healthFill.fillAmount = (float)currentHealth / maxHealth;

		// Calculate the position of the end of the health fill
		var fillEndPos = healthFill.fillAmount * healthFill.rectTransform.rect.width;

		// The new X position for the life icon should be exactly at the end of the fill
		// Assuming the pivot of the lifeIcon is set correctly to be centered horizontally
		var iconNewX = fillEndPos; //TODO: use fillEndPos in line 77? As you don't have any calculations here?

		// Set the life icon's position to the new position
		lifeIcon.rectTransform.anchoredPosition = new Vector2(iconNewX, lifeIcon.rectTransform.anchoredPosition.y);

		// Ensure the life icon is visible even when lives are zero
		lifeIcon.enabled = currentHealth > 0;
	}

	private void CreateDividers()
	{
		var healthBarRectTransform = healthFill.rectTransform.parent.GetComponent<RectTransform>();
		var healthBarWidth = healthBarRectTransform.rect.width;

		// Clear any existing dividers
		foreach (Transform child in healthBarRectTransform)
		{
			if (child.name.Contains("Divider(Clone)"))
			{
				Destroy(child.gameObject);
			}
		}

		for (var i = 1; i < maxHealth; i++)
		{
			var divider = Instantiate(dividerPrefab, healthBarRectTransform);
			var dividerRT = divider.GetComponent<RectTransform>();
			dividerRT.localScale = Vector3.one; // Ensure it has the correct scale
			dividerRT.sizeDelta = new Vector2(4, healthBarRectTransform.rect.height); // Set the divider's width and height

			// Calculate the position based on the actual width of the health bar
			var dividerPositionX = (healthBarWidth / maxHealth) * i - (healthBarWidth * healthBarRectTransform.pivot.x);
			dividerRT.anchoredPosition = new Vector2(dividerPositionX, 0);

			dividerRT.anchorMin = new Vector2(0.5f, 0.5f);
			dividerRT.anchorMax = new Vector2(0.5f, 0.5f);
			dividerRT.pivot = new Vector2(0.5f, 0.5f);
		}
	}
}
