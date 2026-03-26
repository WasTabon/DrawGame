using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class Iteration11_TutorialSetup
{
    [MenuItem("DrawGame/Setup Tutorial on MainMenu (Iteration 11)")]
    public static void SetupTutorial()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            if (!EditorUtility.DisplayDialog("Setup Tutorial",
                "Current scene is '" + scene.name + "'. Are you on the MainMenu scene?",
                "Yes, continue", "Cancel"))
                return;
        }

        var mainMenuUI = Object.FindObjectOfType<MainMenuUI>();
        Debug.Assert(mainMenuUI != null, "MainMenuUI not found! Run Iteration 2 setup first.");

        var tutorialGo = CreateOrGetTutorialPanel(mainMenuUI.transform.parent != null ? mainMenuUI.transform.parent : mainMenuUI.transform);
        if (tutorialGo.transform.parent == mainMenuUI.transform)
        {
            tutorialGo.transform.SetParent(mainMenuUI.gameObject.GetComponentInParent<Canvas>().transform, false);
        }

        var canvasTransform = mainMenuUI.GetComponent<Canvas>() != null
            ? mainMenuUI.transform
            : mainMenuUI.GetComponentInParent<Canvas>().transform;
        tutorialGo.transform.SetParent(canvasTransform, false);
        tutorialGo.transform.SetAsLastSibling();

        var tutorialUI = tutorialGo.GetComponent<TutorialUI>();
        WireTutorialUI(tutorialUI, tutorialGo);
        WireMainMenuUI(mainMenuUI, tutorialUI);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Tutorial setup complete on MainMenu scene!");
    }

    private static GameObject CreateOrGetTutorialPanel(Transform parent)
    {
        var canvas = parent.GetComponentInParent<Canvas>();
        Transform canvasTransform = canvas != null ? canvas.transform : parent;

        var existing = canvasTransform.Find("TutorialPanel");
        if (existing != null)
        {
            Debug.Log("TutorialPanel already exists, updating...");
            return existing.gameObject;
        }

        var panelGo = new GameObject("TutorialPanel");
        panelGo.transform.SetParent(canvasTransform, false);
        var panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        var bg = panelGo.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.12f, 1f);

        var cg = panelGo.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        panelGo.AddComponent<TutorialUI>();

        var slideContainer = new GameObject("SlideContainer");
        slideContainer.transform.SetParent(panelGo.transform, false);
        var slideContainerRect = slideContainer.AddComponent<RectTransform>();
        slideContainerRect.anchorMin = new Vector2(0f, 0.15f);
        slideContainerRect.anchorMax = new Vector2(1f, 0.92f);
        slideContainerRect.offsetMin = Vector2.zero;
        slideContainerRect.offsetMax = Vector2.zero;

        CreateSlide1(slideContainer.transform);
        CreateSlide2(slideContainer.transform);
        CreateSlide3(slideContainer.transform);
        CreateSlide4(slideContainer.transform);

        var pageIndicator = CreatePageIndicator(panelGo.transform);
        var nextButton = CreateNextButton(panelGo.transform);

        return panelGo;
    }

    private static void CreateSlide1(Transform parent)
    {
        var slide = CreateSlideBase(parent, "Slide1");

        CreateSlideTitle(slide.transform, "DRAW WITH YOUR FINGER");
        CreateSlideSubtext(slide.transform, "Swipe across the screen\nto draw a line");
        CreateSlideIllustration(slide.transform, CreateFingerDrawIllustration);
    }

    private static void CreateSlide2(Transform parent)
    {
        var slide = CreateSlideBase(parent, "Slide2");

        CreateSlideTitle(slide.transform, "LINES BECOME OBJECTS");
        CreateSlideSubtext(slide.transform, "When you release, your line\nturns into a physical object and falls");
        CreateSlideIllustration(slide.transform, CreateFallIllustration);
    }

    private static void CreateSlide3(Transform parent)
    {
        var slide = CreateSlideBase(parent, "Slide3");

        CreateSlideTitle(slide.transform, "REACH THE GOAL");
        CreateSlideSubtext(slide.transform, "Push the ball into\nthe green zone to win");
        CreateSlideIllustration(slide.transform, CreateGoalIllustration);
    }

    private static void CreateSlide4(Transform parent)
    {
        var slide = CreateSlideBase(parent, "Slide4");

        CreateSlideTitle(slide.transform, "READY?");
        CreateSlideSubtext(slide.transform, "Use as few lines as possible\nfor more stars!");
        CreateSlideIllustration(slide.transform, CreateStarsIllustration);
    }

    private static GameObject CreateSlideBase(Transform parent, string name)
    {
        var existing = parent.Find(name);
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        go.AddComponent<CanvasGroup>();
        go.SetActive(false);

        return go;
    }

    private static void CreateSlideTitle(Transform parent, string text)
    {
        var go = new GameObject("Title");
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 42;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.05f, 0.78f);
        rect.anchorMax = new Vector2(0.95f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void CreateSlideSubtext(Transform parent, string text)
    {
        var go = new GameObject("Subtext");
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 26;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.7f, 0.7f, 0.75f, 1f);
        tmp.lineSpacing = 10f;
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.08f, 0.62f);
        rect.anchorMax = new Vector2(0.92f, 0.78f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void CreateSlideIllustration(Transform parent, System.Action<Transform> drawFunc)
    {
        var go = new GameObject("Illustration");
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.05f);
        rect.anchorMax = new Vector2(0.9f, 0.6f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        drawFunc(go.transform);
    }

    private static void CreateFingerDrawIllustration(Transform parent)
    {
        var lineGo = new GameObject("Line");
        lineGo.transform.SetParent(parent, false);
        var lineImg = lineGo.AddComponent<Image>();
        lineImg.color = new Color(0.3f, 0.7f, 1f, 0.8f);
        var lineRect = lineGo.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.15f, 0.45f);
        lineRect.anchorMax = new Vector2(0.75f, 0.48f);
        lineRect.offsetMin = Vector2.zero;
        lineRect.offsetMax = Vector2.zero;

        var dotGo = new GameObject("FingerDot");
        dotGo.transform.SetParent(parent, false);
        var dotImg = dotGo.AddComponent<Image>();
        dotImg.color = new Color(1f, 1f, 1f, 0.9f);
        var dotRect = dotGo.GetComponent<RectTransform>();
        dotRect.anchorMin = new Vector2(0.7f, 0.4f);
        dotRect.anchorMax = new Vector2(0.8f, 0.53f);
        dotRect.offsetMin = Vector2.zero;
        dotRect.offsetMax = Vector2.zero;

        var arrowGo = new GameObject("Arrow");
        arrowGo.transform.SetParent(parent, false);
        var arrowTmp = arrowGo.AddComponent<TextMeshProUGUI>();
        arrowTmp.text = "☞";
        arrowTmp.fontSize = 60;
        arrowTmp.alignment = TextAlignmentOptions.Center;
        arrowTmp.color = new Color(1f, 1f, 1f, 0.7f);
        var arrowRect = arrowGo.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(0.65f, 0.2f);
        arrowRect.anchorMax = new Vector2(0.85f, 0.42f);
        arrowRect.offsetMin = Vector2.zero;
        arrowRect.offsetMax = Vector2.zero;
    }

    private static void CreateFallIllustration(Transform parent)
    {
        var lineGo = new GameObject("FrozenLine");
        lineGo.transform.SetParent(parent, false);
        var lineImg = lineGo.AddComponent<Image>();
        lineImg.color = new Color(0.4f, 0.4f, 0.45f, 0.9f);
        var lineRect = lineGo.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0.2f, 0.55f);
        lineRect.anchorMax = new Vector2(0.7f, 0.58f);
        lineRect.offsetMin = Vector2.zero;
        lineRect.offsetMax = Vector2.zero;

        var arrowGo = new GameObject("DownArrow");
        arrowGo.transform.SetParent(parent, false);
        var arrowTmp = arrowGo.AddComponent<TextMeshProUGUI>();
        arrowTmp.text = "▼\n▼\n▼";
        arrowTmp.fontSize = 30;
        arrowTmp.alignment = TextAlignmentOptions.Center;
        arrowTmp.color = new Color(1f, 0.85f, 0.2f, 0.7f);
        arrowTmp.lineSpacing = -20f;
        var arrowRect = arrowGo.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(0.35f, 0.15f);
        arrowRect.anchorMax = new Vector2(0.55f, 0.52f);
        arrowRect.offsetMin = Vector2.zero;
        arrowRect.offsetMax = Vector2.zero;

        var groundGo = new GameObject("Ground");
        groundGo.transform.SetParent(parent, false);
        var groundImg = groundGo.AddComponent<Image>();
        groundImg.color = new Color(0.35f, 0.35f, 0.4f, 0.6f);
        var groundRect = groundGo.GetComponent<RectTransform>();
        groundRect.anchorMin = new Vector2(0.1f, 0.08f);
        groundRect.anchorMax = new Vector2(0.9f, 0.12f);
        groundRect.offsetMin = Vector2.zero;
        groundRect.offsetMax = Vector2.zero;
    }

    private static void CreateGoalIllustration(Transform parent)
    {
        var platformGo = new GameObject("Platform");
        platformGo.transform.SetParent(parent, false);
        var platformImg = platformGo.AddComponent<Image>();
        platformImg.color = new Color(0.4f, 0.4f, 0.45f, 0.9f);
        var platformRect = platformGo.GetComponent<RectTransform>();
        platformRect.anchorMin = new Vector2(0.15f, 0.5f);
        platformRect.anchorMax = new Vector2(0.55f, 0.54f);
        platformRect.offsetMin = Vector2.zero;
        platformRect.offsetMax = Vector2.zero;

        var ballGo = new GameObject("Ball");
        ballGo.transform.SetParent(parent, false);
        var ballTmp = ballGo.AddComponent<TextMeshProUGUI>();
        ballTmp.text = "●";
        ballTmp.fontSize = 50;
        ballTmp.alignment = TextAlignmentOptions.Center;
        ballTmp.color = new Color(1f, 0.5f, 0.2f, 1f);
        var ballRect = ballGo.GetComponent<RectTransform>();
        ballRect.anchorMin = new Vector2(0.27f, 0.54f);
        ballRect.anchorMax = new Vector2(0.43f, 0.72f);
        ballRect.offsetMin = Vector2.zero;
        ballRect.offsetMax = Vector2.zero;

        var arrowGo = new GameObject("Arrow");
        arrowGo.transform.SetParent(parent, false);
        var arrowTmp = arrowGo.AddComponent<TextMeshProUGUI>();
        arrowTmp.text = "→";
        arrowTmp.fontSize = 50;
        arrowTmp.alignment = TextAlignmentOptions.Center;
        arrowTmp.color = new Color(1f, 1f, 1f, 0.6f);
        var arrowRect = arrowGo.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(0.45f, 0.35f);
        arrowRect.anchorMax = new Vector2(0.6f, 0.55f);
        arrowRect.offsetMin = Vector2.zero;
        arrowRect.offsetMax = Vector2.zero;

        var goalGo = new GameObject("GoalZone");
        goalGo.transform.SetParent(parent, false);
        var goalImg = goalGo.AddComponent<Image>();
        goalImg.color = new Color(0.2f, 0.9f, 0.3f, 0.4f);
        var goalRect = goalGo.GetComponent<RectTransform>();
        goalRect.anchorMin = new Vector2(0.6f, 0.12f);
        goalRect.anchorMax = new Vector2(0.85f, 0.42f);
        goalRect.offsetMin = Vector2.zero;
        goalRect.offsetMax = Vector2.zero;

        var goalLabelGo = new GameObject("GoalLabel");
        goalLabelGo.transform.SetParent(goalGo.transform, false);
        var goalLabelTmp = goalLabelGo.AddComponent<TextMeshProUGUI>();
        goalLabelTmp.text = "GOAL";
        goalLabelTmp.fontSize = 20;
        goalLabelTmp.fontStyle = FontStyles.Bold;
        goalLabelTmp.alignment = TextAlignmentOptions.Center;
        goalLabelTmp.color = new Color(1f, 1f, 1f, 0.8f);
        var goalLabelRect = goalLabelGo.GetComponent<RectTransform>();
        goalLabelRect.anchorMin = Vector2.zero;
        goalLabelRect.anchorMax = Vector2.one;
        goalLabelRect.offsetMin = Vector2.zero;
        goalLabelRect.offsetMax = Vector2.zero;
    }

    private static void CreateStarsIllustration(Transform parent)
    {
        var starsGo = new GameObject("Stars");
        starsGo.transform.SetParent(parent, false);
        var starsTmp = starsGo.AddComponent<TextMeshProUGUI>();
        starsTmp.text = "★  ★  ★";
        starsTmp.fontSize = 70;
        starsTmp.alignment = TextAlignmentOptions.Center;
        starsTmp.color = new Color(1f, 0.85f, 0.2f, 1f);
        var starsRect = starsGo.GetComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(0.1f, 0.4f);
        starsRect.anchorMax = new Vector2(0.9f, 0.7f);
        starsRect.offsetMin = Vector2.zero;
        starsRect.offsetMax = Vector2.zero;

        var tipGo = new GameObject("Tip");
        tipGo.transform.SetParent(parent, false);
        var tipTmp = tipGo.AddComponent<TextMeshProUGUI>();
        tipTmp.text = "Less lines = More stars";
        tipTmp.fontSize = 24;
        tipTmp.alignment = TextAlignmentOptions.Center;
        tipTmp.color = new Color(0.6f, 0.6f, 0.65f, 1f);
        var tipRect = tipGo.GetComponent<RectTransform>();
        tipRect.anchorMin = new Vector2(0.1f, 0.2f);
        tipRect.anchorMax = new Vector2(0.9f, 0.38f);
        tipRect.offsetMin = Vector2.zero;
        tipRect.offsetMax = Vector2.zero;
    }

    private static GameObject CreatePageIndicator(Transform parent)
    {
        var existing = parent.Find("PageIndicator");
        if (existing != null) return existing.gameObject;

        var go = new GameObject("PageIndicator");
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "● ○ ○ ○";
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.6f, 0.6f, 0.65f, 1f);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.1f);
        rect.anchorMax = new Vector2(0.8f, 0.15f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return go;
    }

    private static GameObject CreateNextButton(Transform parent)
    {
        var existing = parent.Find("NextButton");
        if (existing != null) return existing.gameObject;

        var btnGo = new GameObject("NextButton");
        btnGo.transform.SetParent(parent, false);
        var btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.25f, 0.02f);
        btnRect.anchorMax = new Vector2(0.75f, 0.09f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.3f, 0.7f, 1f, 1f);

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(btnGo.transform, false);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "NEXT";
        tmp.fontSize = 32;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return btnGo;
    }

    private static void WireTutorialUI(TutorialUI tutorialUI, GameObject tutorialGo)
    {
        var so = new SerializedObject(tutorialUI);

        so.FindProperty("panelGroup").objectReferenceValue = tutorialGo.GetComponent<CanvasGroup>();

        var slideContainer = tutorialGo.transform.Find("SlideContainer");
        so.FindProperty("slideContainer").objectReferenceValue = slideContainer.GetComponent<RectTransform>();

        var nextBtn = tutorialGo.transform.Find("NextButton");
        so.FindProperty("nextButton").objectReferenceValue = nextBtn.GetComponent<Button>();

        var nextBtnText = nextBtn.Find("Text");
        so.FindProperty("nextButtonText").objectReferenceValue = nextBtnText.GetComponent<TextMeshProUGUI>();

        var pageIndicator = tutorialGo.transform.Find("PageIndicator");
        so.FindProperty("pageIndicator").objectReferenceValue = pageIndicator.GetComponent<TextMeshProUGUI>();

        so.ApplyModifiedProperties();
    }

    private static void WireMainMenuUI(MainMenuUI mainMenuUI, TutorialUI tutorialUI)
    {
        var so = new SerializedObject(mainMenuUI);
        so.FindProperty("tutorialUI").objectReferenceValue = tutorialUI;

        var canvas = mainMenuUI.GetComponentInParent<Canvas>() != null
            ? mainMenuUI.GetComponentInParent<Canvas>().transform
            : mainMenuUI.transform;

        var mainMenuPanel = canvas.Find("MainMenuPanel");
        if (mainMenuPanel != null)
            so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel.GetComponent<CanvasGroup>();

        var levelSelectPanel = canvas.Find("LevelSelectPanel");
        if (levelSelectPanel != null)
            so.FindProperty("levelSelectPanel").objectReferenceValue = levelSelectPanel.GetComponent<CanvasGroup>();

        if (mainMenuPanel != null)
        {
            var playBtn = mainMenuPanel.Find("PlayButton");
            if (playBtn != null)
                so.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        }

        so.ApplyModifiedProperties();
    }
}
