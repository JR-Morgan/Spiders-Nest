using System.Collections.Generic;

/// <summary>
/// Encapsulates an <see cref="Enemy"/>'s agency in making AI decisions based on a basic Subsumption architecture
/// </summary>
/// <remarks>See <see cref="EnemyAgentFactory"/></remarks>
public class EnemyAgent
{
    private IList<EnemyBehaviour> behaviours;
    public EnemyAgent(IList<EnemyBehaviour> behaviours)
    {
        this.behaviours = behaviours;
    }

    public void Act()
    {
        var state = new BehaviourState();
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (behaviours[i].Invoke(state).shouldTerminate) break;
        }

    }
}