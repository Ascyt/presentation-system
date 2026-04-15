using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public abstract class AnimatedStateMachine<T> : MonoBehaviour where T : Enum
{
    protected abstract void OnEnterState(T state);
    protected abstract void BeforeExitState(T state);
    protected abstract void OnStart();

    protected virtual void OnUpdate() { }


    private readonly HashSet<KeyCode> _nextKeysDefault = new() { KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.Space, KeyCode.PageDown, KeyCode.Return };
    protected virtual HashSet<KeyCode> NextKeys => _nextKeysDefault;

    private readonly HashSet<KeyCode> _prevKeysDefault = new() { KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.LeftShift, KeyCode.PageUp, KeyCode.Backspace };
    protected virtual HashSet<KeyCode> PrevKeys => _prevKeysDefault;

    protected virtual Dictionary<T, float> AutoSkipStates => new();
    private bool _autoSkipCurrentState = false;
    private float _autoSkipTimeLeft = 0f;

    private T _currentState = default!;
    public T CurrentState { get => _currentState; set => SetState(value); }
    public int StateCount => Enum.GetValues(typeof(T)).Length;

    private Notes notes = null!;

    public delegate void FadeFunc(float fadingValue, bool isExit);
    private struct FadeData
    {
        public Fading fading;
        public FadeFunc func;
        public bool runWhileOnDelay;
        public bool reverse;
    }
    private readonly HashSet<FadeData> _singleFades = new();
    protected void Fade(Fading fading, FadeFunc func, bool runWhileOnDelay=true, bool reverse=false)
    {
        FadeData newData = new()
        {
            fading = fading,
            func = func,
            runWhileOnDelay = runWhileOnDelay,
            reverse = reverse
        };

        _singleFades.Add(newData);

        fading.StartFade();

        if (runWhileOnDelay || fading.delayTimeLeft <= 0f)
            func(reverse ? 1f - newData.fading.value : newData.fading.value, isExit: false);
    }
    protected void FadeAndReturnToZero(Fading fading, FadeFunc func, bool runWhileOnDelay=true)
    {
        Fading fadingA = fading
            .WithDuration(f => f.fadeDuration / 2f);
        if (fadingA.fadeEasing.io == Easing.IO.InOut) 
            fadingA.fadeEasing.io = Easing.IO.In;
        if (fadingA.fadeEasing.io == Easing.IO.Out)
            fadingA.fadeEasing.type = Easing.Type.Linear;

        Fading fadingB = fading
            .WithDuration(f => f.fadeDuration / 2f)
            .WithDelay(f => f.delayDuration + fadingA.fadeDuration);
        if (fadingB.fadeEasing.io == Easing.IO.InOut) 
            fadingB.fadeEasing.io = Easing.IO.Out;
        if (fadingB.fadeEasing.io == Easing.IO.In)
            fadingB.fadeEasing.type = Easing.Type.Linear;

        Fade(fadingA, func, runWhileOnDelay);
        Fade(fadingB, func, runWhileOnDelay, reverse: true);
    }
    public delegate void UpdateFunc(float deltaTime, bool isExit);
    private class UpdateData
    {
        #pragma warning disable CS8618 // "required" keyword doesn't exist in this C# version
        public UpdateFunc func;
        #pragma warning restore CS8618 
    }
    private readonly HashSet<UpdateData> _singleUpdates = new();
    protected void OnStateUpdate(UpdateFunc func)
    {
        UpdateData newData = new()
        {
            func = func
        };

        _singleUpdates.Add(newData);

        func(0f, isExit: false);
    }

    public void SetState(T state)
    {
        T target = state;

        if (!Enum.IsDefined(typeof(T), target))
        {
            Debug.LogError($"Invalid state: {target}.");
            return;
        }
        if (_currentState.Equals(target))
        {
            // Restart current state
            RestartCurrentState();

            return;
        }

        // Exit currently running state
        PrivateBeforeExitState(_currentState);

        // If jumping to a future state: Enter, rush and exit intermediate states from old to new
        int intermediatePastStatesCount = target.ToInt() - _currentState.ToInt() - 1;
        for (int i = 1; i <= intermediatePastStatesCount; i++)
        {
            T pastState = _currentState.RunOnEnumAsInt(s => s + i);

            // Enter intermediate step
            PrivateOnEnterState(pastState);

            // Exit intermediate step
            PrivateBeforeExitState(pastState);
        }
        // If jumping to a past state: Reset all states, run again from start to target
        if (target.ToInt() < _currentState.ToInt())
        {
            // Start first state
            _currentState = default!;
            OnStart();
            PrivateOnEnterState(_currentState);

            // Run through all states until target
            SetState(target);
            return;
        }

        _currentState = target;

        // Start the target state
        PrivateOnEnterState(_currentState);

        if (AutoSkipStates.TryGetValue(_currentState, out float autoSkipTime))
        {
            _autoSkipCurrentState = true;
            _autoSkipTimeLeft = autoSkipTime;
        }
        else
        {
            _autoSkipCurrentState = false;
            _autoSkipTimeLeft = 0f;
        }
    }
    public void NextState()
    {
        SetState(_currentState.RunOnEnumAsInt(s => (s + 1) % StateCount));
        
        if (_currentState.ToString().StartsWith("_"))
        {
            // If the state name starts with "_", skip it
            NextState();
        }
    }
    public void PrevState()
    {
        SetState(_currentState.RunOnEnumAsInt(s => (s - 1 + StateCount) % StateCount));

        // Rush the current state and prevent it from being skipped automatically
        RushCurrentState();
        _autoSkipCurrentState = false;

        if (_currentState.ToString().StartsWith("_"))
        {
            // If the state name starts with "_", skip it
            PrevState();
        }
    }

    public void RestartCurrentState()
    {
        foreach (UpdateData data in _singleUpdates)
        {
            data.func(0f, isExit: false);
        }

        foreach (FadeData data in _singleFades)
        {
            data.fading.StartFade();

            if (data.runWhileOnDelay || data.fading.delayTimeLeft <= 0f)
                data.func(data.reverse ? 1f - data.fading.value : data.fading.value, isExit: false);
        }

        OnEnterState(_currentState);
    }
    public void RushCurrentState()
    {
        foreach (FadeData data in _singleFades)
        {
            data.fading.RushFade();
            data.func(data.reverse ? 1f - data.fading.value : data.fading.value, isExit: true);
        }
        _singleFades.Clear();

        foreach (UpdateData data in _singleUpdates.ToArray())
        {
            data.func(0f, isExit: true);
            _singleUpdates.Remove(data); 
        }
    }
    public void UpdateCurrentState(float deltaTime)
    {
        foreach (FadeData data in _singleFades.ToArray())
        {
            if (data.runWhileOnDelay || data.fading.delayTimeLeft <= 0f)
                data.func(data.reverse ? 1f - data.fading.value : data.fading.value, isExit: false);

            data.fading.UpdateFade(deltaTime);

            if (!data.fading.isFading)
            {
                // If fading is done, remove it from the set
                data.func(data.reverse ? 1f - data.fading.value : data.fading.value, isExit: true);
                _singleFades.Remove(data);
            }
        }

        foreach (UpdateData data in _singleUpdates)
        {
            data.func(deltaTime, isExit: false);
        }
    }
    private void RunCurrentState()
    {
        foreach (FadeData data in _singleFades)
        {
            if (data.runWhileOnDelay || data.fading.delayTimeLeft <= 0f)
                data.func(data.reverse ? 1f - data.fading.value : data.fading.value, isExit: false);
        }

        foreach (UpdateData data in _singleUpdates)
        {
            data.func(0f, isExit: false);
        }
    }

    private void PrivateOnEnterState(T state)
    {
        OnEnterState(state);
        RunCurrentState();
    }
    private void PrivateBeforeExitState(T state)
    {
        RushCurrentState();
        BeforeExitState(state);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (RunPrompt.Instance.isShowing) 
            {
                RunPrompt.Instance.Hide();
            }
            else 
            {
                RunPrompt.Instance.ShowSimpleChoice("Jump to state", Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(
                    state => state.ToString(),
                    state => (Action)(() => SetState(state))
                ));
            }
        }

        if (RunPrompt.Instance.isShowing)
            return;

        if (_autoSkipCurrentState)
        {
            _autoSkipTimeLeft -= Time.deltaTime;

            while (_autoSkipTimeLeft <= 0f && _autoSkipCurrentState)
            {
                _autoSkipCurrentState = false;
                NextState();
            }
        }

        if (NextKeys.Any(key => Input.GetKeyDown(key)))
        {
            NextState();
        }
        if (PrevKeys.Any(key => Input.GetKeyDown(key)))
        {
            PrevState();
        }

        UpdateCurrentState(Time.deltaTime);

        OnUpdate();
    }

    private void Start()
    {
        OnStart();
        PrivateOnEnterState(_currentState);

        if (AutoSkipStates.TryGetValue(_currentState, out float autoSkipTime))
        {
            _autoSkipCurrentState = true;
            _autoSkipTimeLeft = autoSkipTime;
        }
    }

    public void UpdateNotes(string text)
    {
        notes ??= new Notes(typeof(T).Name);

        notes.Set($"[ {CurrentState} ]\n\n" + text);
    }
    public void UpdateNotes()
        => UpdateNotes("");
}
