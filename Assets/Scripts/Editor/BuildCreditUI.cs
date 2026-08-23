using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;

public class BuildCreditUI 
{
    [MenuItem("RaionHackJam/Build Credit UI")]
    public static void BuildUI()
    {
        // 1. Create CreditController
        GameObject managerObj = new GameObject("CreditManager");
        CreditController controller = managerObj.AddComponent<CreditController>();

        // Destroy old canvas if exists
        Canvas oldCanvas = Object.FindObjectOfType<Canvas>();
        if (oldCanvas != null && oldCanvas.name == "Canvas") 
        {
            Object.DestroyImmediate(oldCanvas.gameObject);
        }

        // 2. Create Canvas
        GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Event System
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
            System.Type newModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (newModuleType != null) {
                esObj.AddComponent(newModuleType);
            } else {
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        // 3. Background
        GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        bgObj.transform.SetParent(canvasObj.transform, false);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        UnityEngine.UI.Image bgImg = bgObj.GetComponent<UnityEngine.UI.Image>();
        bgImg.color = new Color(0.1f, 0.12f, 0.15f, 1f);

        // 4. Title
        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        titleObj.transform.SetParent(canvasObj.transform, false);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.9f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(800, 150);
        
        Text titleTxt = titleObj.GetComponent<Text>();
        titleTxt.text = "CREDITS";
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.color = Color.white;
        titleTxt.fontSize = 80;
        titleTxt.fontStyle = FontStyle.Bold;
        
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleTxt.font = defaultFont;
        
        Outline outline = titleObj.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(3, -3);

        // 5. Scroll View for Credits
        GameObject scrollObj = new GameObject("ScrollView", typeof(RectTransform), typeof(CanvasRenderer), typeof(ScrollRect));
        scrollObj.transform.SetParent(canvasObj.transform, false);
        RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRect.pivot = new Vector2(0.5f, 0.5f);
        scrollRect.anchoredPosition = new Vector2(0, -50);
        scrollRect.sizeDelta = new Vector2(1200, 600);

        ScrollRect scrollComp = scrollObj.GetComponent<ScrollRect>();
        scrollComp.horizontal = false; 
        scrollComp.movementType = ScrollRect.MovementType.Elastic;
        scrollComp.scrollSensitivity = 30f;

        // Viewport
        GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Mask));
        viewportObj.transform.SetParent(scrollObj.transform, false);
        RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        
        UnityEngine.UI.Image viewportImg = viewportObj.GetComponent<UnityEngine.UI.Image>();
        viewportImg.color = new Color(1, 1, 1, 0.05f);
        Mask mask = viewportObj.GetComponent<Mask>();
        mask.showMaskGraphic = false;

        // Content
        GameObject contentObj = new GameObject("Content", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(ContentSizeFitter));
        contentObj.transform.SetParent(viewportObj.transform, false);
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 1f);
        contentRect.anchorMax = new Vector2(0.5f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = new Vector2(1200, 0); 

        ContentSizeFitter csf = contentObj.GetComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        Text contentTxt = contentObj.GetComponent<Text>();
        contentTxt.text = "Game created by:\n\nAditya - Programmer & Designer\n\nMade for Raion HackJam 2026\n\nThanks for Playing!\n\n\n\n[ Tambahkan Teks Lebih Banyak Di Sini Untuk Melihat Efek Scroll ]\n\n\n\n...\n\n\n\n...\n\n\n\n...\n\n\n\nSelesai!";
        contentTxt.alignment = TextAnchor.UpperCenter;
        contentTxt.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        contentTxt.fontSize = 40;
        contentTxt.font = defaultFont;

        scrollComp.viewport = viewportRect;
        scrollComp.content = contentRect;

        // 6. Back Button
        GameObject btnObj = new GameObject("BackButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(Button));
        btnObj.transform.SetParent(canvasObj.transform, false);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.1f);
        btnRect.anchorMax = new Vector2(0.5f, 0.1f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = Vector2.zero;
        btnRect.sizeDelta = new Vector2(300, 80);

        UnityEngine.UI.Image img = btnObj.GetComponent<UnityEngine.UI.Image>();
        img.color = new Color(0.8f, 0.3f, 0.3f, 1f);

        Button btn = btnObj.GetComponent<Button>();
        UnityAction action = new UnityAction(controller.BackToMenu);
        UnityEventTools.AddPersistentListener(btn.onClick, action);

        GameObject btnTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        btnTextObj.transform.SetParent(btnObj.transform, false);
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;

        Text btnTxt = btnTextObj.GetComponent<Text>();
        btnTxt.text = "BACK";
        btnTxt.alignment = TextAnchor.MiddleCenter;
        btnTxt.color = Color.white;
        btnTxt.font = defaultFont;
        btnTxt.fontSize = 36;
        btnTxt.fontStyle = FontStyle.Bold;

        Debug.Log("Credit UI set up successfully!");
    }
}
