using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class RunPrompt : MonoBehaviour
{
    public struct OnInputResult
    {
        public string[] options;
        public string errorText;
    }

    public static RunPrompt Instance { get; private set; }

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI optionsText;
    [SerializeField]
    private TextMeshProUGUI errorText;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }

    public delegate OnInputResult OnInputFunc(string input);
    public delegate void OnEnterFunc(string input);
    
    private OnInputFunc onInputFunc;
    private OnEnterFunc onEnterFunc;
    private OnInputResult result;

    public bool isShowing = false;
    private bool hideNextUpdate = false;

    public void ShowSimpleChoice(string title, Dictionary<string, Action> options)
    {
        static bool optionLambda(string input, KeyValuePair<string, Action> o) => 
            string.IsNullOrEmpty(input) || o.Key.ToLower().StartsWith(input.ToLower());

        Show(title, 
            input => new OnInputResult 
            { 
                options = 
                    options
                    .Where(o => optionLambda(input, o))
                    .Select(o => o.Key)
                    .ToArray(),
                errorText = options.Any(o => optionLambda(input, o)) ? null : "No options match your input."
            }, 
            input => 
            {
                options.FirstOrDefault(o => optionLambda(input, o)).Value?.Invoke();
                return;
            });
    }

    public void Show(string title, OnInputFunc onInputFunc, OnEnterFunc onEnterFunc)
    {
        if (isShowing)
            return;

        panel.SetActive(true);

        optionsText.gameObject.SetActive(true);
        optionsText.text = "";
        errorText.gameObject.SetActive(false);

        titleText.text = title;

        inputField.text = "";
        inputField.ActivateInputField();

        this.onInputFunc = onInputFunc;
        this.onEnterFunc = onEnterFunc;

        isShowing = true;

        OnInputChanged();
    }
    public void Hide()
    {
        panel.SetActive(false);
        isShowing = false;
    }

    public void OnInputChanged()
    {
        result = onInputFunc(inputField.text);

        if (!string.IsNullOrEmpty(result.errorText))
        {
            errorText.text = result.errorText;
            errorText.gameObject.SetActive(true);
            optionsText.gameObject.SetActive(false);
            return;
        }

        errorText.gameObject.SetActive(false);
        optionsText.gameObject.SetActive(true);
        if (result.options.Length > 0)
        {
            StringBuilder optionsTextBuilder = new();

            optionsTextBuilder.AppendLine($"<color=yellow>{result.options[0]}</color>");
            for (int i = 1; i < result.options.Length; i++)
            {
                optionsTextBuilder.AppendLine(result.options[i]);
            }

            optionsText.text = optionsTextBuilder.ToString();
        }
        else
        {
            optionsText.text = "";
        }
    }

    void Update()
    {
        if (!isShowing)
            return;

        if (hideNextUpdate)
        {
            hideNextUpdate = false;
            Hide();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (string.IsNullOrEmpty(result.errorText))
            {
                onEnterFunc(inputField.text);
                hideNextUpdate = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hideNextUpdate = true;
        }
    }
}
