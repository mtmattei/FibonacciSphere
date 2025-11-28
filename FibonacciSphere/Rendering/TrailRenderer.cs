using System;
using System.Collections.Generic;
using System.Numerics;
using FibonacciSphere.Models;
using SkiaSharp;

namespace FibonacciSphere.Rendering;

/// <summary>
/// Renders motion trails behind sphere points.
/// </summary>
public class TrailRenderer
{
    private readonly SKPaint _linePaint;
    private readonly SKPaint _dotPaint;

    public TrailRenderer()
    {
        _linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            IsAntialias = true
        };

        _dotPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };
    }

    /// <summary>
    /// Renders the trail for a single point.
    /// </summary>
    public void RenderTrail(SKCanvas canvas, SpherePoint point, SphereSettings settings)
    {
        if (point.TrailHistory.Count < 2 || settings.TrailLength == 0)
        {
            return;
        }

        var trail = point.TrailHistory.ToArray();
        var baseColor = point.Color;

        switch (settings.TrailStyle)
        {
            case TrailStyle.Line:
                RenderLineTrail(canvas, trail, baseColor, settings);
                break;
            case TrailStyle.Dots:
                RenderDotTrail(canvas, trail, baseColor, settings);
                break;
            case TrailStyle.Ribbon:
                RenderRibbonTrail(canvas, trail, baseColor, settings, point.Size);
                break;
        }
    }

    private void RenderLineTrail(SKCanvas canvas, Vector2[] trail, SKColor baseColor, SphereSettings settings)
    {
        if (trail.Length < 2)
        {
            return;
        }

        using var path = new SKPath();
        path.MoveTo(trail[0].X, trail[0].Y);

        for (int i = 1; i < trail.Length; i++)
        {
            float t = (float)i / trail.Length;
            float alpha = settings.TrailOpacity * t * 255f;

            _linePaint.Color = baseColor.WithAlpha((byte)alpha);
            _linePaint.StrokeWidth = MathF.Max(1f, 3f * t);

            canvas.DrawLine(
                trail[i - 1].X, trail[i - 1].Y,
                trail[i].X, trail[i].Y,
                _linePaint);
        }
    }

    private void RenderDotTrail(SKCanvas canvas, Vector2[] trail, SKColor baseColor, SphereSettings settings)
    {
        for (int i = 0; i < trail.Length; i++)
        {
            float t = (float)i / trail.Length;
            float alpha = settings.TrailOpacity * t * 255f;
            float size = MathF.Max(1f, 4f * t);

            _dotPaint.Color = baseColor.WithAlpha((byte)alpha);
            canvas.DrawCircle(trail[i].X, trail[i].Y, size, _dotPaint);
        }
    }

    private void RenderRibbonTrail(SKCanvas canvas, Vector2[] trail, SKColor baseColor, SphereSettings settings, float pointSize)
    {
        if (trail.Length < 2)
        {
            return;
        }

        using var path = new SKPath();
        var points = new List<SKPoint>();

        // Build ribbon outline
        for (int i = 0; i < trail.Length - 1; i++)
        {
            float t = (float)i / trail.Length;
            var current = trail[i];
            var next = trail[i + 1];

            // Calculate perpendicular direction for ribbon width
            var direction = Vector2.Normalize(next - current);
            var perpendicular = new Vector2(-direction.Y, direction.X);

            float width = pointSize * 0.5f * t;
            points.Add(new SKPoint(current.X + perpendicular.X * width, current.Y + perpendicular.Y * width));
        }

        // Add return path
        for (int i = trail.Length - 2; i >= 0; i--)
        {
            float t = (float)i / trail.Length;
            var current = trail[i];
            var next = trail[System.Math.Min(i + 1, trail.Length - 1)];

            var direction = Vector2.Normalize(next - current);
            var perpendicular = new Vector2(-direction.Y, direction.X);

            float width = pointSize * 0.5f * t;
            points.Add(new SKPoint(current.X - perpendicular.X * width, current.Y - perpendicular.Y * width));
        }

        if (points.Count > 2)
        {
            path.MoveTo(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                path.LineTo(points[i]);
            }
            path.Close();

            using var shader = SKShader.CreateLinearGradient(
                new SKPoint(trail[0].X, trail[0].Y),
                new SKPoint(trail[^1].X, trail[^1].Y),
                new SKColor[] { baseColor.WithAlpha(0), baseColor.WithAlpha((byte)(settings.TrailOpacity * 255)) },
                SKShaderTileMode.Clamp);

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Shader = shader,
                IsAntialias = true
            };

            canvas.DrawPath(path, paint);
        }
    }

    public void Dispose()
    {
        _linePaint.Dispose();
        _dotPaint.Dispose();
    }
}
