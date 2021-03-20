# FarmUtils

This is a command line recordkeeping system for dairy farms.
The primary purpose is to record events that happen to cows
and then report on those events later. The primary output
of the program will be csv sent to stdout for further
processing using some formatting or reporting tool.

Example: Alias the cli for readability

```shell
alias enter=farmutils
alias show=farmutils
alias report=farmutils

enter born Cow123 --dam Cow10 --asof TODAY
show Cow123
report CowsToBreed --asof TUESDAY
```

Example: Using aliased cli, run a report and format with csvkit

```shell
report CowsToBreed --asof TUESDAY > /tmp/CowsToBreedTuesday.csv
csvjoin <(cat /tmp/CowsToBreedTuesday.csv) <(echo "'notes              '") | csvlook > CowsToBreedTuesDay.txt

# CowsToBreedTuesday.txt
# | cow  | dslut | notes               |
# | ---- | ----- | ------------------- |
# | Cow1 |     3 |                     |
# | Cow2 |     3 |                     |
```

