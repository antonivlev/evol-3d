# Central Pattern Generator evolution
This Unity project is based on the following paper: http://users.sussex.ac.uk/~philh/pubs/reilhusbandsIEEE.pdf

Here is an early generation of the controller:

![could not fetch image][early]

And here is a later one:

![could not fetch image][late]

As is visible, selecting the fittest parameters for the controller has produced some tendency to move forward (or rather backward), since the fitness is measured by the character's distance from origin. However, the character has not learned to walk yet because the fitness function does not reward cyclic activity, or perhaps because the model is configured to produce unrealistic motion. The next thing to try would be to re-evaluate the model and to modify the fitness function.

And here is a code map of the project showing dependencies between the classes and rough outlines of the methods:

![could not fetch image][map]

[late]: https://github.com/antonivlev/Evol3d/raw/master/Markdown/late.gif "late generation"
[early]: https://github.com/antonivlev/Evol3d/raw/master/Markdown/early.gif "early generation"
[map]: https://github.com/antonivlev/Evol3d/raw/master/Markdown/map.png "code map"
