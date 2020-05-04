﻿using System;

public class InfoEventArgs<T0> : EventArgs {
    public T0 Arg0;

    public InfoEventArgs() {
        Arg0 = default(T0);
    }

    public InfoEventArgs(T0 info) {
        this.Arg0 = info;
    }
}

public class InfoEventArgs<T0, T1> : InfoEventArgs<T0> {
    public T1 Arg1;

    public InfoEventArgs(T0 arg0, T1 arg1) : base(arg0) {
        this.Arg1 = arg1;
    }
}

public class InfoEventArgs<T0, T1, T2> : InfoEventArgs<T0, T1> {
    public T2 Arg2;
    
    public InfoEventArgs(T0 arg0, T1 arg1, T2 arg2) : base(arg0, arg1) {
        this.Arg2 = arg2;
    }
}

public class InfoEventArgs<T0, T1, T2, T3> : InfoEventArgs<T0, T1, T2> {
    public T3 Arg3;
    
    public InfoEventArgs(T0 arg0, T1 arg1, T2 arg2, T3 arg3) : base(arg0, arg1, arg2) {
        this.Arg3 = arg3;
    }
}
