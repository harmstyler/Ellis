using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Blend.Ellis.PageOutputs
{
    public interface IPageOutput
    {
        Hashtable Settings { get; set; }
        void Execute(Page thisPage);
    }
}
