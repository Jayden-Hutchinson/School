#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <ucontext.h>
#include <sys/syscall.h>

#define STACK_SIZE 8192
ucontext_t main_ctx, th1_ctx, th2_ctx;


void thread_1() {
    int c = 0;
    while(1){
        c++;
        printf("Thread 1.\n");        
        if (c%5==0){
            sleep(1);
            swapcontext(&th1_ctx, &th2_ctx);
        }
    }
}

void thread_2() {
    int c = 0;
    while(1){
        c++;
        printf("Thread 2.\n");         
        if (c%3==0){
            sleep(1);
            swapcontext(&th2_ctx, &th1_ctx);
        }
    }
}

int main() {
    // Allocate stacks
    char *sum_stack = malloc(STACK_SIZE);
    char *sort_stack = malloc(STACK_SIZE);

    if (!sum_stack || !sort_stack) {
        perror("malloc");
        exit(1);
    }

    // Create sum thread context
    getcontext(&th1_ctx);
    th1_ctx.uc_link = 0;
    th1_ctx.uc_stack.ss_sp = sum_stack;
    th1_ctx.uc_stack.ss_size = STACK_SIZE;
    makecontext(&th1_ctx, thread_1, 0);

    // Create sort thread context
    getcontext(&th2_ctx);
    th2_ctx.uc_link = 0;
    th2_ctx.uc_stack.ss_sp = sort_stack;
    th2_ctx.uc_stack.ss_size = STACK_SIZE;
    makecontext(&th2_ctx, thread_2, 0);

    setcontext(&th1_ctx);

    free(sum_stack);
    free(sort_stack);
    return 0;
}
