# Raw Accel Guide

## Installation
Run "installer.exe" in the release directory to install the raw accel driver. Restart your computer for the installation to take effect.  

Run "uninstaller.exe" in the release directory to uninstall the driver. Restart for the uninstallation to take effect.

Run "grapher.exe" when the driver is installed in order to run the Raw Accel GUI.

## Philosophy
The Raw Accel driver and GUI's workings and exposed parameters are based on our understanding of mouse acceleration. Our understanding includes the concepts of "gain", "whole vs by component", and "anisotropy." For clarity, we will outline this understanding here. Those uninterested can skip to Features below.

### Measurements from Input Speed
Raw Accel, like any mouse modification program, works by acting on a passed-in (x,y) input in order to pass back out an (x,y) output. The GUI program creates charts by feeding a set of (x,y) inputs to the driver code to receive a set of (x,y) outputs. The following measurements, available as charts in Raw Accel, are then found from the outputs:

- **Sensitivity**: The ratio of the output speed to the input speed. The "sensitivity" parameter in the program is a multiplier used on the post-calculation output vector.
- **(Output) Velocity**: The speed of the final output vector. The output vs input velocity curve is perhaps the most important relationship in a particular setup because it directly describes the output for any given input. (We use "speed" and "velocity" interchangeably, and are aware of the difference elsewhere.)
- **Gain**: The slope of the output vs input velocity curve. It answers the question: "if I move my hand a little faster, how much faster will my cursor move?" The relationship between gain and sensitivity is that if gain is continuous, so is sensitivity. The reverse is not necessarily true, so keeping the gain "nice" ensures "nice" sensitivity but not vice versa.
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

### Offsets
An offset, sometimes called a threshold, is a speed in counts before acceleration "kicks in". The legacy way of applying an offset is having a multiplier of 1 below and at the offset, and applying the sensitivity of (speed-offset) above. This legacy "sensitivity offset" is still available but causes a large discontuinity in gain at the point of offset, leading to non-smooth feeling at offset cross. The new default "gain offset" does a little extra math to simply shift the gain graph by the offset amount without any discontinuity. This feels smoother and has almost no effect on sensitivity. The theory behind "gain offsets" is developed in [this document](https://docs.google.com/document/d/1P6LygpeEazyHfjVmaEygCsyBjwwW2A-eMBl81ZfxXZk).

### Caps
A cap is a point after which acceleration is not applied. The legacy way of applying an offset is simply applying the minimum of the cap sensitivity and the calculated sensitivity for any acceleration calculation. Thus, for the legacy "sensitivity cap" the value given is a sensitivity. This cap style is still available but causes a large discontinuity at the point of offset, leading to a non-smooth feeling at cap cross. The new default "gain cap" effectively caps the gain, but for internal calculation reasons, does so for a given speed rather than a given gain value. This feels much smoother but might have a large effect on sensitivity as gain generally raises faster than sensitivity. We recommend that users use a gain cap and simply adjust it to hit at the gain equivalent to the sensitivity they'd like to cap at. The theory behind "gain caps" is developed in [this document](https://docs.google.com/document/d/1FCpkqRxUaCP7J258SupbxNxvdPfljb16AKMs56yDucA).

### Weight
Our acceleration functions generally have sensitivity functions that start at 1 and then increase. A weight is a multiplier of that increase before it is added to 1. For instance, if we would have had accelerated sensitivity 1.5, weight of 0.5 will result in accelerated sensitivity 1.25 (1 + 0.5\*0.5), and a weight of 3 would have resulted in a sensitivity of 2.5 (1 + 0.5\*3).

Weight is primarily a quick and dirty way to test a new curve. It also can be given a negative value to allow negative acceleration. Most acceleration styles could just change the parameters to have the same affect as setting a weight. Some curves, like the logarithm style, can achieve a greater range of shapes by changing weight.

### By Component & Anisotropy
See "Horizontal and Vertical" in the philosophy section to understand what these do.

### Last Mouse Move
The Raw Accel GUI reads the output of the raw input stream, and thus the output of the Raw Accel Driver, and displays on the graphs red points corresponding to the last mouse movements. These calulations should be fast and your graph responsive, but it comes at the cost of higher CPU usage due to needing to refresh the graph often. This feature can be turned off in the "Charts" menu.

### Scale by DPI and Poll Rate
This option does not scale your acceleration curve in any way. Rather, it scales the set of points used to graph your curve, and shows you a window of input speed relevant for your chosen DPI and Poll Rate. The poll rate is also used to determine the Last Mouse Move points and therefore should be set for accuracy in that measurement.

## Acceleration Styles
The examples of various types below show some typical settings, without a cap or offset, for a mouse at 1200 DPI and 1000 hz.

### Linear
This is simplest style used by most; it is simply a line rising at a given rate. This is a good choice for new users.
![LinearExample](\images\linear_example.png)

### Classic
This is the style found in Quake 3, Quake Live, and countless inspired followers, including the InterAccel program. It mulplies the speed by a given rate and then raises the product to a given exponent. Any particular linear style curve can be replicated in classic style with an exponent of 2.
![ClassicExample](\images\classic_example.png)

### Power
This is the style found in CS:GO and Source Engine games (m_customaccel 3). The user can set a rate by which the speed is multplied, and then an exponent to which the product is raised, which is then the final multiplier (no adding to 1.). In the aforementioned games the default m_customaccel_exponent value of 1.05 would be a value of 0.05 in Raw Accel, leading to a concave slowly rising curve. CS:GO and Source Engine games apply acceleration in an fps-dependent manner, so Raw Accel can only simulate acceleration from these games at a given fps. To do so, set scale to 1000/(in-game fps).
![PowerExample](\images\power_example.png)

### Natural & NaturalGain
Natural is a style found in the game Diabotical. It features a concave curve which starts at 1 and approaches some maximum sensitivity. This style is unique and useful but causes an ugly dip in the gain graph. For this reason we have created the NaturalGain style, which recreates the Natural style shape in the gain graph without any dips. We recommend users use the NaturalGain style instead of the Natural style; on switch some small tweaks may be needed since for any particular settings the NaturalGain is slightly slower to ramp up than the Natural style. NaturalGain is another excellent choice for new users.
![NaturalExample](\images\natural_example.png)
![NaturalGainExample](\images\natural_gain_example.png)

### Motivity
This curve looks like an "S" with the top half bigger than the bottom. Mathematically it's a "Sigmoid function on a log-log plot". A user can set the "midpoint" of the S, the "acceleration" (i.e. slantedness) of the S, and the "motivity". "Motivity" sets min and max sensitivity, where the maximum is just "motivity", and the minimum is "1/motivity." (Sensitivity is 1 at the midpoint.) This curve is calculated and stored in a lookup table before applying acceleration, which makes the gain graph look a little funny.  This is one author's favorite curve, and an excellent choice for power users and new users who don't mind playing with the settings a little.
![MotivityExample](\images\motivity_example.png)

## Further Help
Further help and frequently asked questions can be found in the FAQ.