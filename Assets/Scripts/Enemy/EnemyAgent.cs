using System.Collections.Generic;

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