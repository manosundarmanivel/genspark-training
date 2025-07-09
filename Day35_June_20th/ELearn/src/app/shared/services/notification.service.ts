import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private hubConnection: signalR.HubConnection | null = null;

  private contentNotificationSource = new BehaviorSubject<{ topic: string, courseTitle: string, fileName: string } | null>(null);
  contentNotification$ = this.contentNotificationSource.asObservable();

  private enrollmentNotificationSource = new BehaviorSubject<{ studentName: string, courseTitle: string } | null>(null);
  enrollmentNotification$ = this.enrollmentNotificationSource.asObservable();

  startConnection(token: string) {
    console.log('[SignalR] Initializing connection...');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/notificationHub`, {
        accessTokenFactory: () => token || '',
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('[SignalR] Connected successfully.');
      })
      .catch(err => {
        console.error('[SignalR] Connection error:', err);
      });

    this.registerHandlers();
  }

  private registerHandlers() {
    if (!this.hubConnection) {
      console.warn('[SignalR] Hub connection is null during handler registration.');
      return;
    }

    console.log('[SignalR] Registering notification handlers...');

    this.hubConnection.on('ReceiveContentUploadNotification', (topic: string ,courseTitle: string, fileName: string) => {
      console.log('[SignalR] Received content upload notification:', { topic,courseTitle, fileName });
      this.contentNotificationSource.next({topic, courseTitle, fileName });
    });

    this.hubConnection.on('ReceiveEnrollmentNotification', (studentName: string, courseTitle: string) => {
      console.log('[SignalR] Received enrollment notification:', { studentName, courseTitle });
      this.enrollmentNotificationSource.next({ studentName, courseTitle });
    });

    this.hubConnection.onreconnecting(error => {
      console.warn('[SignalR] Connection lost. Reconnecting...', error);
    });

    this.hubConnection.onreconnected(connectionId => {
      console.log('[SignalR] Reconnected successfully. Connection ID:', connectionId);
    });

    this.hubConnection.onclose(error => {
      console.error('[SignalR] Connection closed.', error);
    });
  }

  joinCourseGroup(courseId: string) {
    if (!this.hubConnection) {
      console.warn('[SignalR] Cannot join group, hub connection not initialized.');
      return;
    }

    console.log(`[SignalR] Joining course group: course-${courseId}`);
    this.hubConnection.invoke('JoinCourseGroup', courseId)
      .catch(err => console.error('[SignalR] Error joining group:', err));
  }

  leaveCourseGroup(courseId: string) {
    if (!this.hubConnection) {
      console.warn('[SignalR] Cannot leave group, hub connection not initialized.');
      return;
    }

    console.log(`[SignalR] Leaving course group: course-${courseId}`);
    this.hubConnection.invoke('LeaveCourseGroup', courseId)
      .catch(err => console.error('[SignalR] Error leaving group:', err));
  }
}
