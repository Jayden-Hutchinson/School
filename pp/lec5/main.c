#include <stdio.h>

#define CHECK(pred) printf("%s ... %s\n", (pred) ? "passed" : "FAILED", #pred)

int main() {
  CHECK(1 + 1 == 2);
  CHECK(1 - 1 == 1);
  return 0;
}
