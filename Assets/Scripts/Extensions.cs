using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static Color Invert(this Color color) {
        return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }
}
