using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBehaviourBase : StateMachineBehaviour
{

    public class Context
    {
        public bool SetupDone;
        public int id;
    }

    protected Context ctx;

    public void Setup(Context _ctx)
    {
        ctx = _ctx;
    }

}
