# Animation of a Cellular Automaton

## Cell

**3 States:**
`alive`
`dying`
`dead`

**On Each Iteration:**

`alive` becomes `dying`

`dying` becomes `dead`

**if exactly 2 alive neighbours**

`dead` becomes `alive`

**else**

`dead`

## Application

### Visuals

`alive` = Black

`dying` = Red

`dead` = White

### controls

**Start / Stop Button**

**Step Button**
allows user to step through the animation one iteration at a time.

**On Load**

New grid of 64x64 cells

Randomly set each cell `alive` or `dead` (may want to use `random` crate for this)
