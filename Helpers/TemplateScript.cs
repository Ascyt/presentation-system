using System.Collections.Generic;
using UnityEngine;

public enum TemplateState
{
    Start
}

[RequireComponent(typeof(Camera))]
public class TemplateScript : AnimatedStateMachine<TemplateState>
{
    private readonly Dictionary<TemplateState, float> _autoSkipStates = new()
    {
    };
    protected override Dictionary<TemplateState, float> AutoSkipStates => _autoSkipStates;

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));
    
    protected override void OnEnterState(TemplateState state)
    {
        switch (state)
        {
            case TemplateState.Start:
                Fade(DefaultFading, 
                    (fadingValue, isExit) =>
                    {
                    });
                return;
        }
    }
    protected override void BeforeExitState(TemplateState state)
    {

    }

    private void Awake()
    {
        
    }

    protected override void OnStart()
    {

    }
}
