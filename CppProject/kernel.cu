
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>
#include <iostream>
using namespace std;

extern "C" 
{
	__global__ void addKernel(int* a, int* b, int* c)
	{
		int i = blockDim.x * blockIdx.x + threadIdx.x;
		c[i] = a[i] + b[i];
	}
}