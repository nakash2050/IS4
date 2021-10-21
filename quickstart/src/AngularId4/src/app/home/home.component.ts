import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs/operators';

import { Logger } from '@core';
import { PersonQuery } from '@app/models/person-query';

import { ApiHttpService } from '@core/services/api-http.service';
import { ApiEndpointsService } from '@core/services/api-endpoints.service';

const log = new Logger('Home');

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  quote: string | undefined;
  isLoading = false;
  webapiData$: any;
  userInfo$: any;
  endpointUrl: any;
  username: string = 'Alice';

  constructor(
    // Application Services
    private apiHttpService: ApiHttpService,
    public apiEndpointsService: ApiEndpointsService
  ) {}

  ngOnInit() {
    log.debug('init');
  }

  callapi() {
    this.isLoading = true;
    this.userInfo$ = undefined;
    this.apiHttpService
      .get(this.apiEndpointsService.getIdentityEndpoint())
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe((resp) => {
        this.webapiData$ = resp;
        log.debug(resp.data);
        log.debug(this.webapiData$);
      });
  }

  getuserInfo() {
    this.isLoading = true;
    this.webapiData$ = undefined;

    this.endpointUrl = this.username === 'Alice' ? this.apiEndpointsService.getUserInfoAliceEndpoint() : 
                       this.username === 'Bob' ? this.apiEndpointsService.getUserInfoBobEndpoint() : this.apiEndpointsService.getUserInfoJanEndpoint();

    this.apiHttpService
      .get(this.endpointUrl)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe(resp => {
        this.userInfo$ = resp;
        console.log(resp);
      }, 
      err => this.userInfo$ = err
    )
  }
}
