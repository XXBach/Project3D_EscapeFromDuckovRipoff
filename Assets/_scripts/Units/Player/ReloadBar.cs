using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ReloadBar : MonoBehaviour
{
    [SerializeField] private Slider _reloadBar;

    private void Awake()
    {
        _reloadBar ??= GetComponent<Slider>();
        HideReloadBar();
        Bus<Reload>.OnEvent += OnReload;
    }
    public void UpdateReloadBar(float progress)
    {
        _reloadBar.value = progress;
    }
    public void ShowReloadBar()
    {
        _reloadBar.gameObject.SetActive(true);
    }
    public void HideReloadBar()
    {
        _reloadBar.gameObject.SetActive(false);
    }
    public void OnReload(Reload reload)
    {
        ShowReloadBar();
        StartCoroutine(ReloadCoroutine(2f)); // Assuming a reload duration of 2 seconds
    }
    public IEnumerator ReloadCoroutine(float reloadDuration)
    {
        float elapsedTime = 0f;
        ShowReloadBar();
        while (elapsedTime < reloadDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / reloadDuration);
            UpdateReloadBar(progress);
            yield return null;
        }
        HideReloadBar();
    }
}
