# binPackingSolver
How This Works:
Sort the Boxes:  
  Arrange the boxes in non-decreasing order of their size.  
Apply First Fit Algorithm:  
  Use the First Fit heuristic to place each box into the bin where it fits.
  This heuristic quickly generates an initial feasible packing solution by trying to minimize wasted space in each bin.  

Refine with Branch and Bound:
  Improve the solution from Step 2 by employing a Branch and Bound method that explores different box rotations and placements.
  This step aims to optimize the packing further by considering various orientations and configurations of the boxes.
Additional Features:
  Cutting Stock Problem: The program can also be adapted to solve cutting stock problems.
Random Fit Option:   
  Multiple solvers can run concurrently with random box orders to explore different potential solutions and find the best one.
Limitations:   
  The program can only handle box placements and does not support spherical objects. The quality of the result is assessed based on space utilization rather than the value of the items.  
app can be found in app folder, use the exe file in it.  
test file also included.
