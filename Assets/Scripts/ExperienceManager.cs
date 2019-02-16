using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExperienceManager {

    public static int GetExpToNextLevel(int _currentLevel)
    {
        return 10 * (_currentLevel + 1);
    }
}
