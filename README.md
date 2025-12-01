# Advent of Code 2025
## `new_project.py`
`new_project.py` is only run on demand, and serves to setup the project for each day. However, it does automatically download the input.txt for each day, and therefore needs to be (and is) compliant with the [automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation) on the [/r/adventofcode](https://www.reddit.com/r/adventofcode/) community wiki.

Specifically:
- The script does not include throttling, as it is only run on demand, and will generally only be used once per day.
- As the tool's purpose is to download the `input.txt` once per day, it has no concept of caching.
- The `User-Agent` header used includes my GitHub user and a description of purpose.