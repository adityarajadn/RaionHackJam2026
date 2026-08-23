using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;

public class BuildPauseUI 
{
    [MenuItem("RaionHackJam/Build Pause UI")]
    public static void BuildUI()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("PauseCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
            System.Type newModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (newModuleType != null) { esObj.AddComponent(newModuleType); } 
            else { esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>(); }
        }

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        PauseMenuController controller = canvas.gameObject.GetComponent<PauseMenuController>();
        if (controller == null) controller = canvas.gameObject.AddComponent<PauseMenuController>();

        // Destroy old ones if exists to prevent duplicates
        Transform oldPanel = canvas.transform.Find("SettingsPanel");
        if (oldPanel) Object.DestroyImmediate(oldPanel.gameObject);
        Transform oldBtn = canvas.transform.Find("PauseButton");
        if (oldBtn) Object.DestroyImmediate(oldBtn.gameObject);

        GameObject panelObj = new GameObject("SettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        panelObj.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        UnityEngine.UI.Image panelImg = panelObj.GetComponent<UnityEngine.UI.Image>();
        panelImg.color = new Color(0, 0, 0, 0.8f);
        panelObj.SetActive(false); 

        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        titleObj.transform.SetParent(panelObj.transform, false);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.sizeDelta = new Vector2(600, 100);
        Text titleTxt = titleObj.GetComponent<Text>();
        titleTxt.text = "SETTINGS (PAUSED)";
        titleTxt.font = defaultFont;
        titleTxt.fontSize = 60;
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.color = Color.white;

        GameObject resumeBtnObj = new GameObject("ResumeButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Button));
        resumeBtnObj.transform.SetParent(panelObj.transform, false);
        RectTransform resRect = resumeBtnObj.GetComponent<RectTransform>();
        resRect.anchorMin = new Vector2(0.5f, 0.5f);
        resRect.anchorMax = new Vector2(0.5f, 0.5f);
        resRect.anchoredPosition = new Vector2(0, 140); 
        resRect.sizeDelta = new Vector2(300, 80);
        UnityEngine.UI.Image resImg = resumeBtnObj.GetComponent<UnityEngine.UI.Image>();
        resImg.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        Button resBtn = resumeBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(resBtn.onClick, new UnityAction(controller.ResumeGame));

        GameObject resTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        resTextObj.transform.SetParent(resumeBtnObj.transform, false);
        RectTransform resTextRect = resTextObj.GetComponent<RectTransform>();
        resTextRect.anchorMin = Vector2.zero;
        resTextRect.anchorMax = Vector2.one;
        resTextRect.sizeDelta = Vector2.zero;
        Text resTxt = resTextObj.GetComponent<Text>();
        resTxt.text = "RESUME";
        resTxt.font = defaultFont;
        resTxt.fontSize = 32;
        resTxt.alignment = TextAnchor.MiddleCenter;
        resTxt.color = Color.white;

        // Music Slider
        GameObject musicSliderObj = DefaultControls.CreateSlider(new DefaultControls.Resources());
        musicSliderObj.name = "MusicSlider";
        musicSliderObj.transform.SetParent(panelObj.transform, false);
        RectTransform musicRect = musicSliderObj.GetComponent<RectTransform>();
        musicRect.anchorMin = new Vector2(0.5f, 0.5f);
        musicRect.anchorMax = new Vector2(0.5f, 0.5f);
        musicRect.anchoredPosition = new Vector2(0, 30);
        musicRect.sizeDelta = new Vector2(300, 30);
        controller.musicSlider = musicSliderObj.GetComponent<Slider>();
        controller.musicSlider.value = 1f;

        GameObject musicLabelObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        musicLabelObj.transform.SetParent(musicSliderObj.transform, false);
        RectTransform musicLabelRect = musicLabelObj.GetComponent<RectTransform>();
        musicLabelRect.anchoredPosition = new Vector2(0, 30);
        Text musicTxt = musicLabelObj.GetComponent<Text>();
        musicTxt.text = "MUSIC VOLUME";
        musicTxt.font = defaultFont;
        musicTxt.fontSize = 24;
        musicTxt.alignment = TextAnchor.MiddleCenter;
        musicTxt.color = Color.white;

        // SFX Slider
        GameObject sfxSliderObj = DefaultControls.CreateSlider(new DefaultControls.Resources());
        sfxSliderObj.name = "SFXSlider";
        sfxSliderObj.transform.SetParent(panelObj.transform, false);
        RectTransform sfxRect = sfxSliderObj.GetComponent<RectTransform>();
        sfxRect.anchorMin = new Vector2(0.5f, 0.5f);
        sfxRect.anchorMax = new Vector2(0.5f, 0.5f);
        sfxRect.anchoredPosition = new Vector2(0, -60);
        sfxRect.sizeDelta = new Vector2(300, 30);
        controller.sfxSlider = sfxSliderObj.GetComponent<Slider>();
        controller.sfxSlider.value = 1f;

        GameObject sfxLabelObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        sfxLabelObj.transform.SetParent(sfxSliderObj.transform, false);
        RectTransform sfxLabelRect = sfxLabelObj.GetComponent<RectTransform>();
        sfxLabelRect.anchoredPosition = new Vector2(0, 30);
        Text sfxTxt = sfxLabelObj.GetComponent<Text>();
        sfxTxt.text = "SFX VOLUME";
        sfxTxt.font = defaultFont;
        sfxTxt.fontSize = 24;
        sfxTxt.alignment = TextAnchor.MiddleCenter;
        sfxTxt.color = Color.white;

        GameObject exitBtnObj = new GameObject("ExitButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Button));
        exitBtnObj.transform.SetParent(panelObj.transform, false);
        RectTransform exitRect = exitBtnObj.GetComponent<RectTransform>();
        exitRect.anchorMin = new Vector2(0.5f, 0.5f);
        exitRect.anchorMax = new Vector2(0.5f, 0.5f);
        exitRect.anchoredPosition = new Vector2(0, -170); // Placed below everything
        exitRect.sizeDelta = new Vector2(300, 80);
        UnityEngine.UI.Image exitImg = exitBtnObj.GetComponent<UnityEngine.UI.Image>();
        exitImg.color = new Color(0.8f, 0.3f, 0.3f, 1f); 
        
        Button exitBtn = exitBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(exitBtn.onClick, new UnityAction(controller.ExitToMainMenu));

        GameObject exitTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        exitTextObj.transform.SetParent(exitBtnObj.transform, false);
        RectTransform exitTextRect = exitTextObj.GetComponent<RectTransform>();
        exitTextRect.anchorMin = Vector2.zero;
        exitTextRect.anchorMax = Vector2.one;
        exitTextRect.sizeDelta = Vector2.zero;
        Text exitTxt = exitTextObj.GetComponent<Text>();
        exitTxt.text = "MAIN MENU";
        exitTxt.font = defaultFont;
        exitTxt.fontSize = 32;
        exitTxt.alignment = TextAnchor.MiddleCenter;
        exitTxt.color = Color.white;

        controller.settingsPanel = panelObj;

        GameObject pauseBtnObj = new GameObject("PauseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Button));
        pauseBtnObj.transform.SetParent(canvas.transform, false);
        pauseBtnObj.transform.SetAsLastSibling(); 
        RectTransform pauseRect = pauseBtnObj.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(0, 1);
        pauseRect.anchorMax = new Vector2(0, 1);
        pauseRect.pivot = new Vector2(0, 1); 
        pauseRect.anchoredPosition = new Vector2(20, -20);
        pauseRect.sizeDelta = new Vector2(100, 100);

        UnityEngine.UI.Image pauseImg = pauseBtnObj.GetComponent<UnityEngine.UI.Image>();
        pauseImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        Button pauseBtn = pauseBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(pauseBtn.onClick, new UnityAction(controller.ToggleSettings));

        GameObject pTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        pTextObj.transform.SetParent(pauseBtnObj.transform, false);
        RectTransform pTextRect = pTextObj.GetComponent<RectTransform>();
        pTextRect.anchorMin = Vector2.zero;
        pTextRect.anchorMax = Vector2.one;
        pTextRect.sizeDelta = Vector2.zero;
        Text pTxt = pTextObj.GetComponent<Text>();
        pTxt.text = "||"; 
        pTxt.font = defaultFont;
        pTxt.fontSize = 40;
        pTxt.alignment = TextAnchor.MiddleCenter;
        pTxt.color = Color.white;

        Debug.Log("Pause UI successfully generated in current Canvas.");
    }
}
