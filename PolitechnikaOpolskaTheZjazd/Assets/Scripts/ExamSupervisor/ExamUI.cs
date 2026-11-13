using UnityEngine;
using UnityEngine.UI;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>Placeholder UI built in code: HUD, professor dialog, result panel.</summary>
    public class ExamUI : MonoBehaviour
    {
        Text _hud;
        Text _dialog;
        Text _result;

        void Awake()
        {
            var canvasGo = new GameObject("ExamCanvas", typeof(Canvas), typeof(CanvasScaler));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);

            _hud = MakeText(canvasGo.transform, "Hud", TextAnchor.UpperLeft, new Vector2(0, 1), new Vector2(20, -20), 28);
            _dialog = MakeText(canvasGo.transform, "Dialog", TextAnchor.LowerCenter, new Vector2(0.5f, 0), new Vector2(0, 40), 32, true);
            _result = MakeText(canvasGo.transform, "Result", TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), Vector2.zero, 36, true);
            _dialog.gameObject.SetActive(false);
            _result.gameObject.SetActive(false);
        }

        static Text MakeText(Transform parent, string name, TextAnchor align, Vector2 anchor, Vector2 pos, int size, bool backdrop = false)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(900, 300);
            var t = go.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size;
            t.alignment = align;
            t.color = Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            if (backdrop) go.AddComponent<Outline>().effectColor = Color.black;
            return t;
        }

        public void SetHud(string text) => _hud.text = text;

        public void ShowDialog(string text) { _dialog.text = text; _dialog.gameObject.SetActive(true); }
        public void HideDialog() => _dialog.gameObject.SetActive(false);

        public void ShowResult(string text) { _result.text = text; _result.gameObject.SetActive(true); }
        public void HideResult() => _result.gameObject.SetActive(false);
    }
}
