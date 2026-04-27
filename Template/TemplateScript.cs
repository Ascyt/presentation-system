using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define your states here.
/// </summary>
/// <remarks>
/// States that start with an underscore (_) will be auto-skipped if not directly switched to.
/// </remarks>
public enum TemplateState
{
    Start,
    End
}

[RequireComponent(typeof(Indexer))]
public class TemplateScript : AnimatedStateMachine<TemplateState>
{
    /// <summary>
    /// Here you can define states that should be automatically skipped after a certain time.
    /// </summary>
    /// <remarks>
    /// This is useful for states that only serve as transitions.
    /// </remarks>
    private readonly Dictionary<TemplateState, float> _autoSkipStates = new()
    {
    };
    protected override Dictionary<TemplateState, float> AutoSkipStates => _autoSkipStates;

    /// <summary>
    /// A default fading for this presentation, so you don't have to specify it every time you call an effect.
    /// </summary>
    private Fading DefaultFading => new(1f, new Easing(Easing.Type.Sine, Easing.IO.InOut));

    /// <summary>
    /// Indexer to recursively access presentation objects by name from the specified entry point.
    /// </summary>
    /// <remarks>
    /// Usage example: I["ParentObject/ChildObject"]
    /// </remarks>
    private Indexer I;
    
    protected override void OnEnterState(TemplateState state)
    {
        switch (state)
        {
            case TemplateState.Start:
                // You can specify notes to use for the presentation.
                // A secondary VS Code window will be opened up with currently set notes.
                // See README.md for details on how to use this in a presentation.
                UpdateNotes("Hello from TemplateScript!");
                return;
            case TemplateState.End:
                UpdateNotes("This is the end state.");

                // Objects can be placed under the "Indexer Entry" GameObject in the scene, and accessed from the script using the Indexer component.
                PresentationObject exampleObject = I["ExampleObject"];

                // There are various built-in effects that you can use. The simplest ones are Fade and FadeTo.
                Effects.FadeTo(this, DefaultFading, exampleObject,
                    position: new Vector2(3, 0), rotation: Quaternion.Euler(0, 0, 180), color: Color.cyan);
                
                // Alternatively, you can use manual fading with fadingValue (0-1) and isExit:
                // Fade(DefaultFading, 
                //     (fadingValue, isExit) =>
                //     {
                        
                //     });
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
        I.ResetToInitial();
    }
}
