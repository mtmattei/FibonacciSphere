using System.Collections.Generic;
using System.Numerics;
using SkiaSharp;

namespace FibonacciSphere.Models;

/// <summary>
/// Represents a single point on the Fibonacci sphere.
/// </summary>
public class SpherePoint
{
    /// <summary>
    /// Index of this point in the sphere.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Base position on the unit sphere (before rotation/wobble).
    /// </summary>
    public Vector3 BasePosition { get; init; }

    /// <summary>
    /// Current animated position (after applying rotation and effects).
    /// </summary>
    public Vector3 CurrentPosition { get; set; }

    /// <summary>
    /// Phase offset for wobble animation to create varied movement.
    /// </summary>
    public float WobblePhase { get; init; }

    /// <summary>
    /// Random direction for random wobble axis mode.
    /// </summary>
    public Vector3 RandomWobbleDirection { get; init; }

    /// <summary>
    /// Current size of the point (may be animated).
    /// </summary>
    public float Size { get; set; }

    /// <summary>
    /// Color of this point.
    /// </summary>
    public SKColor Color { get; set; }

    /// <summary>
    /// Whether this point is currently selected.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Whether the pointer is hovering over this point.
    /// </summary>
    public bool IsHovered { get; set; }

    /// <summary>
    /// History of screen positions for trail rendering.
    /// </summary>
    public Queue<Vector2> TrailHistory { get; } = new();

    /// <summary>
    /// Maximum trail length (set from settings).
    /// </summary>
    public int MaxTrailLength { get; set; } = 20;

    /// <summary>
    /// Depth value for z-ordering (camera Z coordinate after projection).
    /// </summary>
    public float Depth { get; set; }

    /// <summary>
    /// Adds a screen position to the trail history, maintaining max length.
    /// </summary>
    public void AddToTrail(Vector2 screenPosition)
    {
        TrailHistory.Enqueue(screenPosition);
        while (TrailHistory.Count > MaxTrailLength)
        {
            TrailHistory.Dequeue();
        }
    }

    /// <summary>
    /// Clears all trail history.
    /// </summary>
    public void ClearTrail()
    {
        TrailHistory.Clear();
    }
}
