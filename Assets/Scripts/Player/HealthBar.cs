using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFill; // The fillable part of the health bar
    public Image lifeIcon; // The movable icon
    public GameObject dividerPrefab; // The prefab for the divider
    public Transform dividerContainer; // The container to hold dividers
    public int maxLives = 3;
    public int currentLives;

    private void Awake()
    {
        currentLives = maxLives;
        StartCoroutine(CreateDividersAfterDelay());

        // CreateDividers();
        // UpdateHealthBar();
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

    public void LoseLife()
    {
        Debug.Log("My Current Life is:"+currentLives);
        currentLives = Mathf.Max(currentLives - 1, 0); // Ensure lives don't go below 0
        UpdateHealthBar();
    }

    public void GainLife() //never used?
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateHealthBar();
        }
    }

    // private void UpdateHealthBar()
    // {
    //     // Update the health fill amount
    //     healthFill.fillAmount = (float)currentLives / maxLives;
    //
    //     // Move the life icon to the end of the fill
    //     // This assumes the anchor for the life icon is set to the middle center
    //     float iconPos = healthFill.fillAmount * healthFill.rectTransform.rect.width - lifeIcon.rectTransform.rect.width / 2;
    //     lifeIcon.rectTransform.anchoredPosition = new Vector2(iconPos, lifeIcon.rectTransform.anchoredPosition.y);
    //     
    //     // Hide the life icon if no lives are left
    //     lifeIcon.enabled = currentLives > 0;
    // }

    private void UpdateHealthBar()
    {
        // Update the health fill amount
        healthFill.fillAmount = (float)currentLives / maxLives;
    
        // Calculate the position of the end of the health fill
        var fillEndPos = healthFill.fillAmount * healthFill.rectTransform.rect.width;

        // The new X position for the life icon should be exactly at the end of the fill
        // Assuming the pivot of the lifeIcon is set correctly to be centered horizontally
        var iconNewX = fillEndPos; //TODO: use fillEndPos in line 77? As you don't have any calculations here?

        // Set the life icon's position to the new position
        lifeIcon.rectTransform.anchoredPosition = new Vector2(iconNewX, lifeIcon.rectTransform.anchoredPosition.y);

        // Ensure the life icon is visible even when lives are zero
        lifeIcon.enabled = currentLives > 0;
    }


    private void CreateDividers()
    {
        var healthBarRectTransform = healthFill.rectTransform.parent.GetComponent<RectTransform>(); //TODO: TryGetcomponent!!!!! --> cf line below
        //gameObject.TryGetComponent<RectTransform>(out var healthBarRectTransform);
        var healthBarWidth = healthBarRectTransform.rect.width;
        var segmentWidth = healthBarWidth / maxLives;

        // Clear any existing dividers
        foreach (Transform child in healthBarRectTransform)
        {
            if (child.name.Contains("Divider(Clone)"))
            {
                Destroy(child.gameObject);
            }
        }
        var healthFillIndex = healthFill.transform.GetSiblingIndex();

        // Get the sibling index of the LifeIcon, so dividers can be placed just below it

        for (var i = 1; i < maxLives; i++)
        {
            var divider = Instantiate(dividerPrefab, healthBarRectTransform);
            var dividerRT = divider.GetComponent<RectTransform>(); //TODO: TryGetComponent!
            dividerRT.localScale = Vector3.one; // Ensure it has the correct scale
            dividerRT.sizeDelta =
                new Vector2(4, healthBarRectTransform.rect.height); // Set the divider's width and height

            // Set the divider's position
            // Calculate the position based on the actual width of the health bar
            var dividerPositionX = (segmentWidth * i) - (healthBarWidth * healthBarRectTransform.pivot.x);
            dividerRT.anchoredPosition = new Vector2(dividerPositionX, 0);
            
            dividerRT.anchorMin = new Vector2(0.5f, 0.5f);
            dividerRT.anchorMax = new Vector2(0.5f, 0.5f);
            dividerRT.pivot = new Vector2(0.5f, 0.5f);
            
            // Place the dividers just above the health bar background but below the fill
            // dividerRT.SetSiblingIndex(0); // Assuming HealthFill is above the dividers in the hierarchy

            // Set the divider just below the LifeIcon in the hierarchy
            dividerRT.SetSiblingIndex(healthFillIndex + 1);
        }

        // Finally, ensure the HealthFill is on top of the dividers
        lifeIcon.transform.SetAsLastSibling();
    }
}