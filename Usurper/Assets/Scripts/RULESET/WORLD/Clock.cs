// First investigate if this can be implemented this way!!!

// The idea is that by incrementing a in every time a turn ends, we can divide that int into 24 segments.
// These segments is for the 24 hours in a day. We want a sin wave-like function where there is a smooth
// increase and decrease of light over the span of the 24 hours. Check how to do this.

// If the above can be implemented, we can make the MapLighter set it's ambientLightLevel to the hour's x/24 percentage
// If a smoother light transition is desired, we can always just get the percentage of the total turn counts instead...