# Raw Accel Guide

## Philosophy
The Raw Accel driver and GUI's workings and exposed parameters are based on our understanding of mouse acceleration. Our understanding includes the concepts of "gain", "whole vs by component", and "anisotropy." For clarity, we will outline this understanding here.

### Measurements from Input Speed
Raw Accel, like any mouse modification program, works by acting on a passed in (x,y) input and passing back out an (x,y) output. The GUI program creates charts by feeding a set of (x,y) inputs to the driver code to receive a set of (x,y) outputs. The following measurements, available as charts in Raw Accel, are then found from the outputs:

- **Sensitivity**: The ratio of the output speed to the input speed. The "sensitivity" parameter in the program is a multiplier used on the post-calculation output vector.
- **(Output) Velocity**: The speed of the final output vector. The output vs input velocity curve is perhaps the most important relationship in a particular setup because it directly describes the output for any given input. (We use "speed" and "velocity" interchangeably, and are aware of the difference elsewhere.)
- **Gain**: The slope of the output vs input velocity curve.
- For the mathematically inclined: for input speed "v" and Output Velocity f(v), Sensitivity is f(v)/v and Gain is f'(v) = d/dv(f(v)).

Acceleration, then, is a characteristic of the velocity curve, defined as true when the velocity curve is non-linear for any input speed.

### Example
The above is much more clear with an example. Let's say I have 
- linear acceleration with acceleration parameter of 0.01
- a sensitivity parameter of 0.5

and I move my mouse to create an input of (30,40) at a poll rate of 1000 hz.

Then our input speed is sqrt(30^2 + 40^2) = 50 counts/ms. Our accelerated sensitivity is calculated to be (1 + 0.1 \* 50) * 0.5 = 1.5 \* 0.5 = 0.75. So our output velocity is 0.75 \* 50  = 37.5. If I run the previous calculations with input speed 49.9 I get output velocity 37.40005, so our gain is about (37.5-37.40005)/(50-49.9) = 0.9995. Here is a picture of the charts in Raw Accel showing the same thing:

![SensVelocityGainExample](\images\accel_readme_example.png)

### Horizontal and Vertical
If you wish, horizontal and vertical components of a mouse movement can be treated completely separately in Raw Accel. In the above example, they are not treated separately; rather they are "combined" by using the magnitude if the input vector: *sqrt(30^2 + 40^2) = 50 counts/ms*. This is called "Whole" application because the whole speed of the input is used. Application styles include:

- Whole: magnitude of vector is input to sensitivity calculation, and applied to whole vector, as in example above.
    - (out_x, out_y) = (in_x\*sens_x, in_y\*sens_y) \* f(sqrt(in_x^2 + in_y^2)), where f(v) is our sensitvity function
    - Separate horizontal and vertical sensitivites still feel great in this mode.
- By Component: The horizontal components are separated and each is given as input to the sensitivity calculation to multiplied by itself before being recombined at output.
    - (out_x, out_y) = (in_x \* f(in_x) \* sens_x, in_y \* f(in_y) \* sens_y))
- Anisotropy: This is a sub-mode of By Component in which the x and y can have separate sensitivity curves.
    - (out_x, out_y) = (in_x \* f(in_x) \* sens_x, in_y \* g(in_y) \* sens_y)) where g(v) is some other sensitivity function.

The authors of this program feel that Whole is the best style for most users, but that users who play games which have very separate horizontal and vertical actions to manage (such as tracking an enemy and controlling recoil) might benefit from trying By Component or Anisotropy.
  
  
## Features

### Gain Offsets

If one applies an offset