# -*- coding: utf-8 -*-
"""
Created on Sat Jan 14 19:16:50 2023

@author: Daniel
"""
import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from scipy.stats import f_oneway

RESULTS_BASIC = "results_basic.csv"
RESULTS_MODIFIED = "results_modified_threshold.csv"
RESULTS_MODIFIED_2 = "results_modified_threshold_and_bidding.csv"
GRAPHICS_PATH = "Graphics/"

SIMULATION_NAME_COLUMN = "simulation_name"

def main():
    create_graphics("a", RESULTS_BASIC)
    create_graphics("b", RESULTS_MODIFIED)
    create_graphics("c", RESULTS_MODIFIED_2)
    perform_anova_test()
    
    
def perform_anova_test():
    columns = ["total_tasks", "total_changes", "total_reloads"]
    basic_df = pd.read_csv(RESULTS_BASIC)
    print(basic_df["drone2_threshold"].describe())
    modified_df = pd.read_csv(RESULTS_MODIFIED)
    print(modified_df["drone2_threshold"].describe())
    modified_2_df = pd.read_csv(RESULTS_MODIFIED_2)
    print(modified_2_df["drone2_threshold"].describe())
    
    basic_df = basic_df[basic_df.index % 179 == 0]
    basic_df = basic_df[basic_df.index != 0]
    basic_df = basic_df.head(15)
    print(basic_df[columns].describe())
    
    modified_df = modified_df[modified_df.index % 179 == 0]
    modified_df = modified_df[modified_df.index != 0]
    modified_df = modified_df.head(15)
    print(modified_df[columns].describe())
    
    modified_2_df = modified_2_df[modified_2_df.index % 179 == 0]
    modified_2_df = modified_2_df[modified_2_df.index != 0]
    modified_2_df = modified_2_df.head(15)
    
    print(modified_2_df[columns].describe())
    
    result = f_oneway(basic_df["total_tasks"], 
                      modified_df["total_tasks"],
                      modified_2_df["total_tasks"]
                      )
    print("ANOVA test for the tasks:", result)
    
    result = f_oneway(basic_df["total_changes"], 
                      modified_df["total_changes"],
                      modified_2_df["total_changes"])
    print("ANOVA test for the changes:", result)
    
    result = f_oneway(basic_df["total_reloads"], 
                      modified_df["total_reloads"],
                      modified_2_df["total_reloads"])
    print("ANOVA test for the reloads:", result)
    
def create_graphics(improved, fname):
    basic_df = pd.read_csv(fname)
    experiments_list = basic_df.groupby(SIMULATION_NAME_COLUMN)
    exp_number = 1
    for sim_name, experiment_df in experiments_list:
        if(exp_number <= 6):
            plot_experiment(improved, experiment_df, exp_number)
            exp_number += 1
        

    
def plot_experiment(t, experiment_df, exp_number):
    plot_cells(t, experiment_df, exp_number)
    plot_specialities(t, experiment_df,exp_number)
    plot_drones(t, experiment_df,exp_number)
    plot_results(t, experiment_df,exp_number)



def plot_results(t, experiment_df, exp_number):
    steps = list(range(0,180))
    fig, ax = plt.subplots()

    ax.plot(steps, 
            experiment_df["total_tasks"],
            label="Completed tasks")
    
    ax.plot(steps, 
            experiment_df["total_changes"],
            label="Changes")
    
    ax.plot(steps,
            experiment_df["total_reloads"],
            label="Recharges")
    
    
    ax.legend()
    
    plt.xlabel("Steps")
    plt.ylabel("Count")
    
    title = f"Experiment {exp_number}{t}: Results"
    # plt.title(title)
    plt.savefig(f"{GRAPHICS_PATH}results{exp_number}{t}.png")
    plt.show()
    return


def plot_cells(t, experiment_df, exp_number):
    steps = list(range(0,180))
    fig, ax = plt.subplots()

    ax.plot(steps, 
            experiment_df["empty_cells"],
            label="Empty")
    
    ax.plot(steps, 
            experiment_df["seed_cells"],
            label="Seed")
    
    ax.plot(steps,
            experiment_df["watered_cells"],
            label="Grown")
    
    
    ax.legend()
    
    plt.xlabel("Steps")
    plt.ylabel("Count")
    
    title = f"Experiment {exp_number}{t}: Cell count"
        
    # plt.title(title)
    plt.savefig(f"{GRAPHICS_PATH}cells{exp_number}{t}.png")
    plt.show()
    return


def plot_drones(t, experiment_df, exp_number):
    steps = list(range(0,180))
    fig, ax = plt.subplots()
    
    for i in range(0,2):
        drone_label = f"Drone {i+1}"
        drone_column = f"drone{i}_threshold"        
        
        ax.plot(steps, 
            experiment_df[drone_column],
            label=drone_label)
    
    
    ax.legend()
    
    plt.xlabel("Steps")
    plt.ylabel("Min threshold")
    
    title = f"Experiment {exp_number}{t}: Drones' thresholds"
    # plt.title(title)
    plt.savefig(f"{GRAPHICS_PATH}thresholds{exp_number}{t}.png")
    plt.show()
    
    return


def plot_specialities(t, experiment_df, exp_number):
    steps = list(range(0,180))
    fig, ax = plt.subplots()
    
    columns = []
    
    for i in range(0,6):
        drone_column = f"drone{i}_speciality"
        columns.append(drone_column)
        
    seeders = []
    waterers = []
    harvesters = []
    
    for index, row in experiment_df.iterrows():
        count = {1:0, 2:0, 3:0}
        for c in columns:
            count[row[c]] += 1
            
        seeders.append(count[1])
        waterers.append(count[2])
        harvesters.append(count[3])
    
    ax.plot(steps,
            seeders,
            label="Plant")
    
    ax.plot(steps,
            waterers,
            label="Water")
    
    ax.plot(steps,
            harvesters,
            label="Harvest")
    
    ax.legend()
    
    plt.xlabel("Steps")
    plt.ylabel("Drones specialized")
    
    
    title = f"Experiment {exp_number}{t}: Drones' specialities"
    # plt.title(title)
    plt.savefig(f"{GRAPHICS_PATH}specialities{exp_number}{t}.png")
    plt.show()
    
    return

if __name__ == "__main__":
    main()