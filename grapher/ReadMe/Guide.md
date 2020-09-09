# Raw Accel Guide

## Philosophy
The Raw Accel driver and GUI's workings and exposed parameters are based on our understanding of mouse acceleration. Our understanding includes the concepts of "gain", "whole vs component", and "anisotropy." For clarity, we will outline this understanding here.
- **Sensitivity**: The ratio of the output speed to the input speed. The "sensitivity" parameter in the program is a multiplier used on the post-calculation output vector.
- **(Output) Velocity**: The speed of the final output vector. The output vs input velocity curve is perhaps the most important relationship in a particular setup because it directly describes the output for any given input.
- **Gain**: The slope of the output vs input velocity curve.
- **Acceleration**: Acceleration is characteristic of a velocity curve. A velocity curve has "acceleration" if at any point the curve is nonlinear. A nonlinear curve results from a non-constant gain and results in a non-constant sensitivity.

### Example
