using System;
using System.Collections.Generic;
using System.Numerics;
using FibonacciSphere.Models;
using FibonacciSphere.Rendering;

namespace FibonacciSphere.Helpers;

/// <summary>
/// Provides hit testing functionality for detecting which point is under the cursor.
/// </summary>
public static class HitTesting
{
    /// <summary>
    /// Finds the nearest point to the given screen position within a tolerance.
    /// </summary>
    /// <param name="points">All sphere points</param>
    /// <param name="screenPosition">Position in screen coordinates</param>
    /// <param name="screenSize">Screen dimensions</param>
    /// <param name="camera">Camera for projection</param>
    /// <param name="tolerance">Maximum distance to consider a hit (in pixels)</param>
    /// <returns>The nearest point if within tolerance, null otherwise</returns>
    public static SpherePoint? FindNearestPoint(
        IEnumerable<SpherePoint> points,
        Vector2 screenPosition,
        Vector2 screenSize,
        Camera3D camera,
        float tolerance = 20f)
    {
        SpherePoint? nearest = null;
        float minDistance = tolerance;
        float nearestDepth = float.MaxValue;

        foreach (var point in points)
        {
            var (projectedPos, depth) = camera.ProjectToScreen(point.CurrentPosition, screenSize);

            float distance = Vector2.Distance(screenPosition, projectedPos);

            // Check if this point is closer (prioritize by screen distance, then by depth)
            if (distance < minDistance || (distance < minDistance + 5f && depth < nearestDepth))
            {
                // Consider the point's visual size for hit testing
                float hitRadius = point.Size + 5f;
                if (distance <= hitRadius || distance < minDistance)
                {
                    minDistance = distance;
                    nearestDepth = depth;
                    nearest = point;
                }
            }
        }

        return nearest;
    }

    /// <summary>
    /// Finds all points within a radius of the given screen position.
    /// </summary>
    /// <param name="points">All sphere points</param>
    /// <param name="screenPosition">Center position in screen coordinates</param>
    /// <param name="screenSize">Screen dimensions</param>
    /// <param name="camera">Camera for projection</param>
    /// <param name="radius">Selection radius in pixels</param>
    /// <returns>List of points within the radius</returns>
    public static List<SpherePoint> FindPointsInRadius(
        IEnumerable<SpherePoint> points,
        Vector2 screenPosition,
        Vector2 screenSize,
        Camera3D camera,
        float radius)
    {
        var result = new List<SpherePoint>();

        foreach (var point in points)
        {
            var (projectedPos, _) = camera.ProjectToScreen(point.CurrentPosition, screenSize);
            float distance = Vector2.Distance(screenPosition, projectedPos);

            if (distance <= radius)
            {
                result.Add(point);
            }
        }

        return result;
    }

    /// <summary>
    /// Finds all points within a rectangular region.
    /// </summary>
    /// <param name="points">All sphere points</param>
    /// <param name="topLeft">Top-left corner of selection rectangle</param>
    /// <param name="bottomRight">Bottom-right corner of selection rectangle</param>
    /// <param name="screenSize">Screen dimensions</param>
    /// <param name="camera">Camera for projection</param>
    /// <returns>List of points within the rectangle</returns>
    public static List<SpherePoint> FindPointsInRectangle(
        IEnumerable<SpherePoint> points,
        Vector2 topLeft,
        Vector2 bottomRight,
        Vector2 screenSize,
        Camera3D camera)
    {
        var result = new List<SpherePoint>();

        // Normalize rectangle coordinates
        float minX = MathF.Min(topLeft.X, bottomRight.X);
        float maxX = MathF.Max(topLeft.X, bottomRight.X);
        float minY = MathF.Min(topLeft.Y, bottomRight.Y);
        float maxY = MathF.Max(topLeft.Y, bottomRight.Y);

        foreach (var point in points)
        {
            var (projectedPos, _) = camera.ProjectToScreen(point.CurrentPosition, screenSize);

            if (projectedPos.X >= minX && projectedPos.X <= maxX &&
                projectedPos.Y >= minY && projectedPos.Y <= maxY)
            {
                result.Add(point);
            }
        }

        return result;
    }

    /// <summary>
    /// Checks if a screen position is within the canvas bounds.
    /// </summary>
    public static bool IsInBounds(Vector2 position, Vector2 screenSize)
    {
        return position.X >= 0 && position.X <= screenSize.X &&
               position.Y >= 0 && position.Y <= screenSize.Y;
    }
}
