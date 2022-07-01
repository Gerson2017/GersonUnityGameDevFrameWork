using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{

    public class TestArchitecure : Architecture<TestArchitecure>
    {


        protected override void Init()
        {
            this.RegisterUtility<IUtility>(new TestUitility());
        }
    }
}
