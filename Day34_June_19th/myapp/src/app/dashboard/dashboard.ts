import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { UsMapComponent } from "../us-map/us-map";

interface User {
  id: number;
  firstName: string;
  lastName: string;
  gender: string;
  role: string;
  state: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, UsMapComponent]
})
export class Dashboard implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];

  genderFilter: string = '';
  roleFilter: string = '';
  stateFilter: string = '';

  genderOptions: string[] = [];
  roleOptions: string[] = [];
  stateOptions: string[] = [];

  genderChartData = {
    labels: ['Male', 'Female'],
    datasets: [{ data: [0, 0] }]
  };

  roleChartData = {
    labels: [] as string[],
    datasets: [{ data: [] as number[], label: 'Roles' }]
  };

  stateChartData = {
  labels: [] as string[],
  datasets: [{ data: [] as number[], label: 'States' }]
};

stateChartType: ChartType = 'bar';
stateChartOptions = {
  indexAxis: 'y' as const, 
  responsive: true
};

  genderChartType: ChartType = 'pie';
  roleChartType: ChartType = 'bar';


  yourMapData = {
  "CA": {  color: "#ff0000" },
  "TX": {  color: "#00ff00" },
  
};

customTooltipFn = (name: string, data: any) => {
  if (!data) return `<h4>${name}</h4><p>No data available</p>`;
  
  return `<h4>${name}</h4><table>
   
  </table>`;
};

  ngOnInit() {
    this.getAllUsers();
  }

  getAllUsers() {
    fetch('https://dummyjson.com/users')
      .then(res => res.json())
      .then(data => {
        this.users = data.users.map((user: any) => ({
          id: user.id,
          firstName: user.firstName,
          lastName: user.lastName,
          gender: user.gender,
          role:  user.role || 'Unknown',
          state: user.address?.state || 'Unknown'
        }));

        this.genderOptions = [...new Set(this.users.map(u => u.gender))] as string[];
        this.roleOptions = [...new Set(this.users.map(u => u.role))] as string[];
        this.stateOptions = [...new Set(this.users.map(u => u.state))] as string[];

        this.filteredUsers = [...this.users];
        this.updateCharts();
      });
  }

filterUsers() {
  this.filteredUsers = this.users.filter(user =>
    (!this.genderFilter || user.gender === this.genderFilter) &&
    (!this.roleFilter || user.role === this.roleFilter) &&
    (!this.stateFilter || user.state === this.stateFilter)
  );
  this.updateCharts();
}



get filteredGenderOptions(): string[] {
  return [...new Set(
    this.users
      .filter(u =>
        (!this.roleFilter || u.role === this.roleFilter) &&
        (!this.stateFilter || u.state === this.stateFilter)
      )
      .map(u => u.gender)
  )];
}

get filteredRoleOptions(): string[] {
  return [...new Set(
    this.users
      .filter(u =>
        (!this.genderFilter || u.gender === this.genderFilter) &&
        (!this.stateFilter || u.state === this.stateFilter)
      )
      .map(u => u.role)
  )];
}

get filteredStateOptions(): string[] {
  return [...new Set(
    this.users
      .filter(u =>
        (!this.genderFilter || u.gender === this.genderFilter) &&
        (!this.roleFilter || u.role === this.roleFilter)
      )
      .map(u => u.state)
  )];
}

get filteredStates(): string[] {
  return this.filteredUsers.map(u => u.state);
}





  resetFilters() {
    this.genderFilter = '';
    this.roleFilter = '';
    this.stateFilter = '';
    this.filteredUsers = [...this.users];
    this.updateCharts();
  }

  updateCharts() {
    const maleCount = this.filteredUsers.filter(u => u.gender === 'male').length;
    const femaleCount = this.filteredUsers.filter(u => u.gender === 'female').length;

    const roles = this.roleOptions;
    const roleCounts = roles.map(role => this.filteredUsers.filter(u => u.role === role).length);
    const states = this.stateOptions;
    const stateCounts = states.map(state => this.filteredUsers.filter(u => u.state === state).length);

    this.genderChartData = {
      labels: ['Male', 'Female'],
      datasets: [{ data: [maleCount, femaleCount] }]
    };

    this.roleChartData = {
      labels: roles,
      datasets: [{ data: roleCounts, label: 'Roles' }]
    };




  }
}
