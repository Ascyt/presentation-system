using System.Collections.Generic;
using UnityEngine;

public enum TemplateState
{
    Start
}

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Indexer))]
public class TemplateScript : AnimatedStateMachine<TemplateState>
{
    private readonly Dictionary<TemplateState, float> _autoSkipStates = new()
    {
    };
    protected override Dictionary<TemplateState, float> AutoSkipStates => _autoSkipStates;

    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));

    private Indexer I;
    
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
        I = GetComponent<Indexer>();
    }

    protected override void OnStart()
    {

    }
}
