﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//       LottieGen version:
//           7.1.1+g046e9eb0a2
//       
//       Command:
//           LottieGen -Language CSharp -Public -WinUIVersion 3.0 -InputFile Done.json
//       
//       Input file:
//           Done.json (7875 bytes created 22:36+09:00 Jun 25 2023)
//       
//       LottieGen source:
//           http://aka.ms/Lottie
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// ____________________________________
// |       Object stats       | Count |
// |__________________________|_______|
// | All CompositionObjects   |   116 |
// |--------------------------+-------|
// | Expression animators     |    20 |
// | KeyFrame animators       |    12 |
// | Reference parameters     |    20 |
// | Expression operations    |     0 |
// |--------------------------+-------|
// | Animated brushes         |     - |
// | Animated gradient stops  |     - |
// | ExpressionAnimations     |     9 |
// | PathKeyFrameAnimations   |     - |
// |--------------------------+-------|
// | ContainerVisuals         |     1 |
// | ShapeVisuals             |     1 |
// |--------------------------+-------|
// | ContainerShapes          |     1 |
// | CompositionSpriteShapes  |     5 |
// |--------------------------+-------|
// | Brushes                  |     3 |
// | Gradient stops           |     - |
// | CompositionVisualSurface |     - |
// ------------------------------------
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI.Composition;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Graphics;
using Windows.UI;

namespace AnimatedVisuals
{
    // Name:        Comp 1
    // Frame rate:  24 fps
    // Frame count: 60
    // Duration:    2500.0 mS
    public sealed class Done
        : Microsoft.UI.Xaml.Controls.IAnimatedVisualSource
    {
        // Animation duration: 2.500 seconds.
        internal const long c_durationTicks = 25000000;

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor)
        {
            object ignored = null;
            return TryCreateAnimatedVisual(compositor, out ignored);
        }

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor, out object diagnostics)
        {
            diagnostics = null;

            var res = 
                new Done_AnimatedVisual(
                    compositor
                    );
                res.CreateAnimations();
                return res;
        }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public double FrameCount => 60d;

        /// <summary>
        /// Gets the frame rate of the animation.
        /// </summary>
        public double Framerate => 24d;

        /// <summary>
        /// Gets the duration of the animation.
        /// </summary>
        public TimeSpan Duration => TimeSpan.FromTicks(c_durationTicks);

        /// <summary>
        /// Converts a zero-based frame number to the corresponding progress value denoting the
        /// start of the frame.
        /// </summary>
        public double FrameToProgress(double frameNumber)
        {
            return frameNumber / 60d;
        }

        /// <summary>
        /// Returns a map from marker names to corresponding progress values.
        /// </summary>
        public IReadOnlyDictionary<string, double> Markers =>
            new Dictionary<string, double>
            {
            };

        /// <summary>
        /// Sets the color property with the given name, or does nothing if no such property
        /// exists.
        /// </summary>
        public void SetColorProperty(string propertyName, Color value)
        {
        }

        /// <summary>
        /// Sets the scalar property with the given name, or does nothing if no such property
        /// exists.
        /// </summary>
        public void SetScalarProperty(string propertyName, double value)
        {
        }

        sealed class Done_AnimatedVisual : Microsoft.UI.Xaml.Controls.IAnimatedVisual2
        {
            const long c_durationTicks = 25000000;
            readonly Compositor _c;
            readonly ExpressionAnimation _reusableExpressionAnimation;
            CompositionColorBrush _colorBrush_AlmostDeepSkyBlue_FF00A5FF;
            CompositionContainerShape _containerShape;
            CompositionEllipseGeometry _ellipse_131p469_1;
            CompositionEllipseGeometry _ellipse_131p469_2;
            CompositionEllipseGeometry _ellipse_131p469_3;
            CompositionPathGeometry _pathGeometry;
            ContainerVisual _root;
            CubicBezierEasingFunction _cubicBezierEasingFunction_0;
            CubicBezierEasingFunction _cubicBezierEasingFunction_1;
            CubicBezierEasingFunction _cubicBezierEasingFunction_2;
            CubicBezierEasingFunction _cubicBezierEasingFunction_3;
            CubicBezierEasingFunction _cubicBezierEasingFunction_4;
            ExpressionAnimation _rootProgress;
            ScalarKeyFrameAnimation _scalarAnimation_0_to_0;
            StepEasingFunction _holdThenStepEasingFunction;
            StepEasingFunction _stepThenHoldEasingFunction;

            static void StartProgressBoundAnimation(
                CompositionObject target,
                string animatedPropertyName,
                CompositionAnimation animation,
                ExpressionAnimation controllerProgressExpression)
            {
                target.StartAnimation(animatedPropertyName, animation);
                var controller = target.TryGetAnimationController(animatedPropertyName);
                controller.Pause();
                controller.StartAnimation("Progress", controllerProgressExpression);
            }

            void BindProperty(
                CompositionObject target,
                string animatedPropertyName,
                string expression,
                string referenceParameterName,
                CompositionObject referencedObject)
            {
                _reusableExpressionAnimation.ClearAllParameters();
                _reusableExpressionAnimation.Expression = expression;
                _reusableExpressionAnimation.SetReferenceParameter(referenceParameterName, referencedObject);
                target.StartAnimation(animatedPropertyName, _reusableExpressionAnimation);
            }

            ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(float initialProgress, float initialValue, CompositionEasingFunction initialEasingFunction)
            {
                var result = _c.CreateScalarKeyFrameAnimation();
                result.Duration = TimeSpan.FromTicks(c_durationTicks);
                result.InsertKeyFrame(initialProgress, initialValue, initialEasingFunction);
                return result;
            }

            CompositionSpriteShape CreateSpriteShape(CompositionGeometry geometry, Matrix3x2 transformMatrix)
            {
                var result = _c.CreateSpriteShape(geometry);
                result.TransformMatrix = transformMatrix;
                return result;
            }

            CompositionSpriteShape CreateSpriteShape(CompositionGeometry geometry, Matrix3x2 transformMatrix, CompositionBrush fillBrush)
            {
                var result = _c.CreateSpriteShape(geometry);
                result.TransformMatrix = transformMatrix;
                result.FillBrush = fillBrush;
                return result;
            }

            CanvasGeometry Geometry()
            {
                CanvasGeometry result;
                using (var builder = new CanvasPathBuilder(null))
                {
                    builder.BeginFigure(new Vector2(-134F, 56F));
                    builder.AddCubicBezier(new Vector2(-134F, 56F), new Vector2(-96F, 109F), new Vector2(-74F, 108F));
                    builder.AddCubicBezier(new Vector2(-52F, 107F), new Vector2(72F, -56F), new Vector2(72F, -56F));
                    builder.EndFigure(CanvasFigureLoop.Open);
                    result = CanvasGeometry.CreatePath(builder);
                }
                return result;
            }

            CompositionColorBrush ColorBrush_AlmostDeepSkyBlue_FF00A5FF()
            {
                return (_colorBrush_AlmostDeepSkyBlue_FF00A5FF == null)
                    ? _colorBrush_AlmostDeepSkyBlue_FF00A5FF = _c.CreateColorBrush(Color.FromArgb(0xFF, 0x00, 0xA5, 0xFF))
                    : _colorBrush_AlmostDeepSkyBlue_FF00A5FF;
            }

            // - Layer aggregator
            // Scale:1.70654,1.70654, Offset:<448.531, 368.531>
            CompositionColorBrush ColorBrush_AlmostGoldenrod_FFF5A623()
            {
                return _c.CreateColorBrush(Color.FromArgb(0xFF, 0xF5, 0xA6, 0x23));
            }

            // - Layer aggregator
            // Offset:<415, 382>
            CompositionColorBrush ColorBrush_White()
            {
                return _c.CreateColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            }

            // Layer aggregator
            CompositionContainerShape ContainerShape()
            {
                if (_containerShape != null) { return _containerShape; }
                var result = _containerShape = _c.CreateContainerShape();
                result.CenterPoint = new Vector2(-54.5309982F, 27.4689999F);
                result.Offset = new Vector2(448.531006F, 368.531006F);
                // ShapeGroup: Ellipse 1 Offset:<-54.531, 27.469>
                result.Shapes.Add(SpriteShape_0());
                return result;
            }

            // - - Layer aggregator
            // ShapeGroup: Ellipse 1 Offset:<-54.531, 27.469>
            // Ellipse Path 1.EllipseGeometry
            CompositionEllipseGeometry Ellipse_131p469_0()
            {
                var result = _c.CreateEllipseGeometry();
                result.Radius = new Vector2(131.468994F, 131.468994F);
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            CompositionEllipseGeometry Ellipse_131p469_1()
            {
                if (_ellipse_131p469_1 != null) { return _ellipse_131p469_1; }
                var result = _ellipse_131p469_1 = _c.CreateEllipseGeometry();
                var propertySet = result.Properties;
                propertySet.InsertScalar("TEnd", 0F);
                propertySet.InsertScalar("TStart", 0F);
                result.Radius = new Vector2(131.468994F, 131.468994F);
                BindProperty(_ellipse_131p469_1, "TrimStart", "Min(my.TStart,my.TEnd)", "my", _ellipse_131p469_1);
                BindProperty(_ellipse_131p469_1, "TrimEnd", "Max(my.TStart,my.TEnd)", "my", _ellipse_131p469_1);
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            CompositionEllipseGeometry Ellipse_131p469_2()
            {
                if (_ellipse_131p469_2 != null) { return _ellipse_131p469_2; }
                var result = _ellipse_131p469_2 = _c.CreateEllipseGeometry();
                var propertySet = result.Properties;
                propertySet.InsertScalar("TEnd", 0F);
                propertySet.InsertScalar("TStart", 0F);
                result.Radius = new Vector2(131.468994F, 131.468994F);
                BindProperty(_ellipse_131p469_2, "TrimStart", "Min(my.TStart,my.TEnd)", "my", _ellipse_131p469_2);
                BindProperty(_ellipse_131p469_2, "TrimEnd", "Max(my.TStart,my.TEnd)", "my", _ellipse_131p469_2);
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            CompositionEllipseGeometry Ellipse_131p469_3()
            {
                if (_ellipse_131p469_3 != null) { return _ellipse_131p469_3; }
                var result = _ellipse_131p469_3 = _c.CreateEllipseGeometry();
                var propertySet = result.Properties;
                propertySet.InsertScalar("TEnd", 0F);
                propertySet.InsertScalar("TStart", 0F);
                result.Radius = new Vector2(131.468994F, 131.468994F);
                BindProperty(_ellipse_131p469_3, "TrimStart", "Min(my.TStart,my.TEnd)", "my", _ellipse_131p469_3);
                BindProperty(_ellipse_131p469_3, "TrimEnd", "Max(my.TStart,my.TEnd)", "my", _ellipse_131p469_3);
                return result;
            }

            CompositionPathGeometry PathGeometry()
            {
                if (_pathGeometry != null) { return _pathGeometry; }
                var result = _pathGeometry = _c.CreatePathGeometry(new CompositionPath(Geometry()));
                var propertySet = result.Properties;
                propertySet.InsertScalar("TEnd", 0F);
                propertySet.InsertScalar("TStart", 0F);
                BindProperty(_pathGeometry, "TrimStart", "Min(my.TStart,my.TEnd)", "my", _pathGeometry);
                BindProperty(_pathGeometry, "TrimEnd", "Max(my.TStart,my.TEnd)", "my", _pathGeometry);
                return result;
            }

            // - Layer aggregator
            // Ellipse Path 1
            CompositionSpriteShape SpriteShape_0()
            {
                // Offset:<-54.531, 27.469>
                var geometry = Ellipse_131p469_0();
                var result = CreateSpriteShape(geometry, new Matrix3x2(1F, 0F, 0F, 1F, -54.5309982F, 27.4689999F), ColorBrush_AlmostDeepSkyBlue_FF00A5FF());;
                return result;
            }

            // Layer aggregator
            // Ellipse Path 1
            CompositionSpriteShape SpriteShape_1()
            {
                // Offset:<394, 396>, Rotation:-0.008976353503701148 degrees, Scale:<1.91952, 1.91952>
                var result = CreateSpriteShape(Ellipse_131p469_1(), new Matrix3x2(1.91952002F, 0F, 0F, 1.91952002F, 394F, 396F));;
                result.StrokeBrush = ColorBrush_AlmostDeepSkyBlue_FF00A5FF();
                result.StrokeDashCap = CompositionStrokeCap.Round;
                result.StrokeStartCap = CompositionStrokeCap.Round;
                result.StrokeEndCap = CompositionStrokeCap.Round;
                result.StrokeThickness = 4F;
                return result;
            }

            // Layer aggregator
            // Ellipse Path 1
            CompositionSpriteShape SpriteShape_2()
            {
                // Offset:<394, 396>, Scale:<1.70654, 1.70654>
                var result = CreateSpriteShape(Ellipse_131p469_2(), new Matrix3x2(1.70653999F, 0F, 0F, 1.70653999F, 394F, 396F));;
                result.StrokeBrush = ColorBrush_AlmostGoldenrod_FFF5A623();
                result.StrokeDashCap = CompositionStrokeCap.Round;
                result.StrokeStartCap = CompositionStrokeCap.Round;
                result.StrokeEndCap = CompositionStrokeCap.Round;
                result.StrokeThickness = 4F;
                return result;
            }

            // Layer aggregator
            // Ellipse Path 1
            CompositionSpriteShape SpriteShape_3()
            {
                // Offset:<394, 396>, Scale:<1.52399, 1.52399>
                var result = CreateSpriteShape(Ellipse_131p469_3(), new Matrix3x2(1.52399004F, 0F, 0F, 1.52399004F, 394F, 396F));;
                result.StrokeBrush = ColorBrush_AlmostDeepSkyBlue_FF00A5FF();
                result.StrokeDashCap = CompositionStrokeCap.Round;
                result.StrokeStartCap = CompositionStrokeCap.Round;
                result.StrokeEndCap = CompositionStrokeCap.Round;
                result.StrokeThickness = 4F;
                return result;
            }

            // Layer aggregator
            // Path 1
            CompositionSpriteShape SpriteShape_4()
            {
                // Offset:<415, 382>, Rotation:-0.007890744035679575 degrees, Scale:<0.85449, 0.85449>
                var result = CreateSpriteShape(PathGeometry(), new Matrix3x2(0.854489982F, 0F, 0F, 0.854489982F, 415F, 382F));;
                result.StrokeBrush = ColorBrush_White();
                result.StrokeDashCap = CompositionStrokeCap.Round;
                result.StrokeStartCap = CompositionStrokeCap.Round;
                result.StrokeEndCap = CompositionStrokeCap.Round;
                result.StrokeMiterLimit = 2F;
                result.StrokeThickness = 55F;
                return result;
            }

            // The root of the composition.
            ContainerVisual Root()
            {
                if (_root != null) { return _root; }
                var result = _root = _c.CreateContainerVisual();
                var propertySet = result.Properties;
                propertySet.InsertScalar("Progress", 0F);
                // Layer aggregator
                result.Children.InsertAtTop(ShapeVisual_0());
                return result;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_0()
            {
                return (_cubicBezierEasingFunction_0 == null)
                    ? _cubicBezierEasingFunction_0 = _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0F, 1F))
                    : _cubicBezierEasingFunction_0;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_1()
            {
                return (_cubicBezierEasingFunction_1 == null)
                    ? _cubicBezierEasingFunction_1 = _c.CreateCubicBezierEasingFunction(new Vector2(0.166999996F, 0F), new Vector2(0F, 1F))
                    : _cubicBezierEasingFunction_1;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_2()
            {
                return (_cubicBezierEasingFunction_2 == null)
                    ? _cubicBezierEasingFunction_2 = _c.CreateCubicBezierEasingFunction(new Vector2(1F, 0F), new Vector2(0.666999996F, 1F))
                    : _cubicBezierEasingFunction_2;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_3()
            {
                return (_cubicBezierEasingFunction_3 == null)
                    ? _cubicBezierEasingFunction_3 = _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0.0410000011F, 1F))
                    : _cubicBezierEasingFunction_3;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_4()
            {
                return (_cubicBezierEasingFunction_4 == null)
                    ? _cubicBezierEasingFunction_4 = _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0.0930000022F, 1F))
                    : _cubicBezierEasingFunction_4;
            }

            ExpressionAnimation RootProgress()
            {
                if (_rootProgress != null) { return _rootProgress; }
                var result = _rootProgress = _c.CreateExpressionAnimation("_.Progress");
                result.SetReferenceParameter("_", _root);
                return result;
            }

            // Scale
            ScalarKeyFrameAnimation ScalarAnimation_0_to_0()
            {
                // Frame 0.
                if (_scalarAnimation_0_to_0 != null) { return _scalarAnimation_0_to_0; }
                var result = _scalarAnimation_0_to_0 = CreateScalarKeyFrameAnimation(0F, 0F, HoldThenStepEasingFunction());
                // Frame 16.
                result.InsertKeyFrame(0.266666681F, 1.41834998F, CubicBezierEasingFunction_0());
                // Frame 27.
                result.InsertKeyFrame(0.449999988F, 1.41834998F, CubicBezierEasingFunction_1());
                // Frame 43.
                result.InsertKeyFrame(0.716666639F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TEnd
            ScalarKeyFrameAnimation TEndScalarAnimation_0_to_0_0()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 6.
                result.InsertKeyFrame(0.100000001F, 0F, HoldThenStepEasingFunction());
                // Frame 21.
                result.InsertKeyFrame(0.349999994F, 1F, CubicBezierEasingFunction_0());
                // Frame 32.
                result.InsertKeyFrame(0.533333361F, 1F, CubicBezierEasingFunction_1());
                // Frame 47.
                result.InsertKeyFrame(0.783333361F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TEnd
            ScalarKeyFrameAnimation TEndScalarAnimation_0_to_0_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 2.
                result.InsertKeyFrame(0.0333333351F, 0F, HoldThenStepEasingFunction());
                // Frame 17.
                result.InsertKeyFrame(0.283333331F, 1F, CubicBezierEasingFunction_0());
                // Frame 29.
                result.InsertKeyFrame(0.483333319F, 1F, CubicBezierEasingFunction_1());
                // Frame 44.
                result.InsertKeyFrame(0.733333349F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TEnd
            ScalarKeyFrameAnimation TEndScalarAnimation_0_to_0_2()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, HoldThenStepEasingFunction());
                // Frame 12.
                result.InsertKeyFrame(0.200000003F, 1F, CubicBezierEasingFunction_4());
                // Frame 27.
                result.InsertKeyFrame(0.449999988F, 1F, CubicBezierEasingFunction_4());
                // Frame 39.
                result.InsertKeyFrame(0.649999976F, 0F, _c.CreateCubicBezierEasingFunction(new Vector2(0.907000005F, 0F), new Vector2(0.666999996F, 1F)));
                return result;
            }

            // TEnd
            ScalarKeyFrameAnimation TEndScalarAnimation_0_to_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 5.
                result.InsertKeyFrame(0.0833333358F, 0F, HoldThenStepEasingFunction());
                // Frame 22.
                result.InsertKeyFrame(0.366666675F, 1F, CubicBezierEasingFunction_0());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TrimOffset
            ScalarKeyFrameAnimation TrimOffsetScalarAnimation_0_to_0_0()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 4.
                result.InsertKeyFrame(0.0666666701F, 0F, HoldThenStepEasingFunction());
                // Frame 16.
                result.InsertKeyFrame(0.266666681F, 1F, CubicBezierEasingFunction_0());
                // Frame 30.
                result.InsertKeyFrame(0.5F, 1F, CubicBezierEasingFunction_1());
                // Frame 42.
                result.InsertKeyFrame(0.699999988F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TrimOffset
            ScalarKeyFrameAnimation TrimOffsetScalarAnimation_0_to_0_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, HoldThenStepEasingFunction());
                // Frame 12.
                result.InsertKeyFrame(0.200000003F, 1F, CubicBezierEasingFunction_0());
                // Frame 27.
                result.InsertKeyFrame(0.449999988F, 1F, CubicBezierEasingFunction_1());
                // Frame 39.
                result.InsertKeyFrame(0.649999976F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TStart
            ScalarKeyFrameAnimation TStartScalarAnimation_0_to_0_0()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 10.
                result.InsertKeyFrame(0.166666672F, 0F, HoldThenStepEasingFunction());
                // Frame 24.
                result.InsertKeyFrame(0.400000006F, 1F, CubicBezierEasingFunction_0());
                // Frame 36.
                result.InsertKeyFrame(0.600000024F, 1F, CubicBezierEasingFunction_1());
                // Frame 50.
                result.InsertKeyFrame(0.833333313F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TStart
            ScalarKeyFrameAnimation TStartScalarAnimation_0_to_0_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 6.
                result.InsertKeyFrame(0.100000001F, 0F, HoldThenStepEasingFunction());
                // Frame 20.
                result.InsertKeyFrame(0.333333343F, 1F, CubicBezierEasingFunction_0());
                // Frame 33.
                result.InsertKeyFrame(0.550000012F, 1F, CubicBezierEasingFunction_1());
                // Frame 47.
                result.InsertKeyFrame(0.783333361F, 0F, CubicBezierEasingFunction_2());
                return result;
            }

            // Ellipse Path 1.EllipseGeometry
            // TStart
            ScalarKeyFrameAnimation TStartScalarAnimation_0_to_0_2()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 4.
                result.InsertKeyFrame(0.0666666701F, 0F, HoldThenStepEasingFunction());
                // Frame 15.
                result.InsertKeyFrame(0.25F, 1F, CubicBezierEasingFunction_3());
                // Frame 31.
                result.InsertKeyFrame(0.516666651F, 1F, CubicBezierEasingFunction_3());
                // Frame 42.
                result.InsertKeyFrame(0.699999988F, 0F, _c.CreateCubicBezierEasingFunction(new Vector2(0.958999991F, 0F), new Vector2(0.666999996F, 1F)));
                return result;
            }

            // TStart
            ScalarKeyFrameAnimation TStartScalarAnimation_0_to_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, StepThenHoldEasingFunction());
                // Frame 22.
                result.InsertKeyFrame(0.366666675F, 0F, HoldThenStepEasingFunction());
                // Frame 36.
                result.InsertKeyFrame(0.600000024F, 1F, _c.CreateCubicBezierEasingFunction(new Vector2(0.463F, 0F), new Vector2(0.852999985F, 1F)));
                return result;
            }

            // Layer aggregator
            ShapeVisual ShapeVisual_0()
            {
                var result = _c.CreateShapeVisual();
                result.Size = new Vector2(800F, 800F);
                var shapes = result.Shapes;
                shapes.Add(ContainerShape());
                // Scale:1.91952,1.91952, Offset:<448.531, 368.531>
                shapes.Add(SpriteShape_1());
                // Scale:1.70654,1.70654, Offset:<448.531, 368.531>
                shapes.Add(SpriteShape_2());
                // Scale:1.52399,1.52399, Offset:<448.531, 368.531>
                shapes.Add(SpriteShape_3());
                // Offset:<415, 382>
                shapes.Add(SpriteShape_4());
                return result;
            }

            StepEasingFunction HoldThenStepEasingFunction()
            {
                if (_holdThenStepEasingFunction != null) { return _holdThenStepEasingFunction; }
                var result = _holdThenStepEasingFunction = _c.CreateStepEasingFunction();
                result.IsFinalStepSingleFrame = true;
                return result;
            }

            StepEasingFunction StepThenHoldEasingFunction()
            {
                if (_stepThenHoldEasingFunction != null) { return _stepThenHoldEasingFunction; }
                var result = _stepThenHoldEasingFunction = _c.CreateStepEasingFunction();
                result.IsInitialStepSingleFrame = true;
                return result;
            }

            internal Done_AnimatedVisual(
                Compositor compositor
                )
            {
                _c = compositor;
                _reusableExpressionAnimation = compositor.CreateExpressionAnimation();
                Root();
            }

            public Visual RootVisual => _root;
            public TimeSpan Duration => TimeSpan.FromTicks(c_durationTicks);
            public Vector2 Size => new Vector2(800F, 800F);
            void IDisposable.Dispose() => _root?.Dispose();

            public void CreateAnimations()
            {
                StartProgressBoundAnimation(_containerShape, "Scale.X", ScalarAnimation_0_to_0(), RootProgress());
                StartProgressBoundAnimation(_containerShape, "Scale.Y", ScalarAnimation_0_to_0(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_1, "TStart", TStartScalarAnimation_0_to_0_0(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_1, "TEnd", TEndScalarAnimation_0_to_0_0(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_1, "TrimOffset", TrimOffsetScalarAnimation_0_to_0_0(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_2, "TStart", TStartScalarAnimation_0_to_0_1(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_2, "TEnd", TEndScalarAnimation_0_to_0_1(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_2, "TrimOffset", TrimOffsetScalarAnimation_0_to_0_1(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_3, "TStart", TStartScalarAnimation_0_to_0_2(), RootProgress());
                StartProgressBoundAnimation(_ellipse_131p469_3, "TEnd", TEndScalarAnimation_0_to_0_2(), RootProgress());
                StartProgressBoundAnimation(_pathGeometry, "TStart", TStartScalarAnimation_0_to_1(), RootProgress());
                StartProgressBoundAnimation(_pathGeometry, "TEnd", TEndScalarAnimation_0_to_1(), RootProgress());
            }

            public void DestroyAnimations()
            {
                _containerShape.StopAnimation("Scale.X");
                _containerShape.StopAnimation("Scale.Y");
                _ellipse_131p469_1.StopAnimation("TStart");
                _ellipse_131p469_1.StopAnimation("TEnd");
                _ellipse_131p469_1.StopAnimation("TrimOffset");
                _ellipse_131p469_2.StopAnimation("TStart");
                _ellipse_131p469_2.StopAnimation("TEnd");
                _ellipse_131p469_2.StopAnimation("TrimOffset");
                _ellipse_131p469_3.StopAnimation("TStart");
                _ellipse_131p469_3.StopAnimation("TEnd");
                _pathGeometry.StopAnimation("TStart");
                _pathGeometry.StopAnimation("TEnd");
            }

        }
    }
}
