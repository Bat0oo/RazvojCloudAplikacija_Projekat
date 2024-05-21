<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Portfolio" generation="1" functional="0" release="0" Id="88119b5d-8bcb-4970-9761-5b2095fa50ff" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="PortfolioGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="HealthStatusService:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Portfolio/PortfolioGroup/LB:HealthStatusService:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="PortfolioService:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Portfolio/PortfolioGroup/LB:PortfolioService:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="HealthMonitoringService:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapHealthMonitoringService:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthMonitoringService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapHealthMonitoringService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthMonitoringServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapHealthMonitoringServiceInstances" />
          </maps>
        </aCS>
        <aCS name="HealthStatusService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapHealthStatusService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="HealthStatusServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapHealthStatusServiceInstances" />
          </maps>
        </aCS>
        <aCS name="NotificationService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapNotificationService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="NotificationServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapNotificationServiceInstances" />
          </maps>
        </aCS>
        <aCS name="PortfolioService:DataConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapPortfolioService:DataConnectionString" />
          </maps>
        </aCS>
        <aCS name="PortfolioService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapPortfolioService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="PortfolioServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Portfolio/PortfolioGroup/MapPortfolioServiceInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:HealthStatusService:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Portfolio/PortfolioGroup/HealthStatusService/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:PortfolioService:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Portfolio/PortfolioGroup/PortfolioService/Endpoint1" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:HealthMonitoringService:NotificationHealth">
          <toPorts>
            <inPortMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringService/NotificationHealth" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:HealthMonitoringService:PortfolioHealth">
          <toPorts>
            <inPortMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringService/PortfolioHealth" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:NotificationService:HealthCheck">
          <toPorts>
            <inPortMoniker name="/Portfolio/PortfolioGroup/NotificationService/HealthCheck" />
          </toPorts>
        </sFSwitchChannel>
        <sFSwitchChannel name="SW:PortfolioService:HealthCheck">
          <toPorts>
            <inPortMoniker name="/Portfolio/PortfolioGroup/PortfolioService/HealthCheck" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapHealthMonitoringService:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringService/DataConnectionString" />
          </setting>
        </map>
        <map name="MapHealthMonitoringService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapHealthMonitoringServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringServiceInstances" />
          </setting>
        </map>
        <map name="MapHealthStatusService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Portfolio/PortfolioGroup/HealthStatusService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapHealthStatusServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/HealthStatusServiceInstances" />
          </setting>
        </map>
        <map name="MapNotificationService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Portfolio/PortfolioGroup/NotificationService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapNotificationServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/NotificationServiceInstances" />
          </setting>
        </map>
        <map name="MapPortfolioService:DataConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Portfolio/PortfolioGroup/PortfolioService/DataConnectionString" />
          </setting>
        </map>
        <map name="MapPortfolioService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Portfolio/PortfolioGroup/PortfolioService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapPortfolioServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/PortfolioServiceInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="HealthMonitoringService" generation="1" functional="0" release="0" software="C:\Users\Urke\Documents\GitHub\RazvojCloudAplikacija_Projekat\Portfolio\Portfolio\csx\Debug\roles\HealthMonitoringService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="NotificationHealth" protocol="tcp" />
              <inPort name="PortfolioHealth" protocol="tcp" />
              <outPort name="HealthMonitoringService:NotificationHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:NotificationHealth" />
                </outToChannel>
              </outPort>
              <outPort name="HealthMonitoringService:PortfolioHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:PortfolioHealth" />
                </outToChannel>
              </outPort>
              <outPort name="NotificationService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:NotificationService:HealthCheck" />
                </outToChannel>
              </outPort>
              <outPort name="PortfolioService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:PortfolioService:HealthCheck" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;HealthMonitoringService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;NotificationHealth&quot; /&gt;&lt;e name=&quot;PortfolioHealth&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PortfolioService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Portfolio/PortfolioGroup/HealthMonitoringServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="HealthStatusService" generation="1" functional="0" release="0" software="C:\Users\Urke\Documents\GitHub\RazvojCloudAplikacija_Projekat\Portfolio\Portfolio\csx\Debug\roles\HealthStatusService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="8081" />
              <outPort name="HealthMonitoringService:NotificationHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:NotificationHealth" />
                </outToChannel>
              </outPort>
              <outPort name="HealthMonitoringService:PortfolioHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:PortfolioHealth" />
                </outToChannel>
              </outPort>
              <outPort name="NotificationService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:NotificationService:HealthCheck" />
                </outToChannel>
              </outPort>
              <outPort name="PortfolioService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:PortfolioService:HealthCheck" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;HealthStatusService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;NotificationHealth&quot; /&gt;&lt;e name=&quot;PortfolioHealth&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PortfolioService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/HealthStatusServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Portfolio/PortfolioGroup/HealthStatusServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Portfolio/PortfolioGroup/HealthStatusServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="NotificationService" generation="1" functional="0" release="0" software="C:\Users\Urke\Documents\GitHub\RazvojCloudAplikacija_Projekat\Portfolio\Portfolio\csx\Debug\roles\NotificationService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="HealthCheck" protocol="tcp" />
              <outPort name="HealthMonitoringService:NotificationHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:NotificationHealth" />
                </outToChannel>
              </outPort>
              <outPort name="HealthMonitoringService:PortfolioHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:PortfolioHealth" />
                </outToChannel>
              </outPort>
              <outPort name="NotificationService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:NotificationService:HealthCheck" />
                </outToChannel>
              </outPort>
              <outPort name="PortfolioService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:PortfolioService:HealthCheck" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;NotificationService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;NotificationHealth&quot; /&gt;&lt;e name=&quot;PortfolioHealth&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PortfolioService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/NotificationServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Portfolio/PortfolioGroup/NotificationServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Portfolio/PortfolioGroup/NotificationServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="PortfolioService" generation="1" functional="0" release="0" software="C:\Users\Urke\Documents\GitHub\RazvojCloudAplikacija_Projekat\Portfolio\Portfolio\csx\Debug\roles\PortfolioService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
              <inPort name="HealthCheck" protocol="tcp" />
              <outPort name="HealthMonitoringService:NotificationHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:NotificationHealth" />
                </outToChannel>
              </outPort>
              <outPort name="HealthMonitoringService:PortfolioHealth" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:HealthMonitoringService:PortfolioHealth" />
                </outToChannel>
              </outPort>
              <outPort name="NotificationService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:NotificationService:HealthCheck" />
                </outToChannel>
              </outPort>
              <outPort name="PortfolioService:HealthCheck" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Portfolio/PortfolioGroup/SW:PortfolioService:HealthCheck" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="DataConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;PortfolioService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;HealthMonitoringService&quot;&gt;&lt;e name=&quot;NotificationHealth&quot; /&gt;&lt;e name=&quot;PortfolioHealth&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;HealthStatusService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;NotificationService&quot;&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;PortfolioService&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;HealthCheck&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Portfolio/PortfolioGroup/PortfolioServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Portfolio/PortfolioGroup/PortfolioServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Portfolio/PortfolioGroup/PortfolioServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="PortfolioServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="HealthStatusServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="NotificationServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="HealthMonitoringServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="HealthMonitoringServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="HealthStatusServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="NotificationServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="PortfolioServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="HealthMonitoringServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="HealthStatusServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="NotificationServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="PortfolioServiceInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="31c51876-0ac5-400d-8c49-7a0889a2cb7f" ref="Microsoft.RedDog.Contract\ServiceContract\PortfolioContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="8203bc21-47ca-4138-b1d3-ce9ea95cbfa9" ref="Microsoft.RedDog.Contract\Interface\HealthStatusService:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Portfolio/PortfolioGroup/HealthStatusService:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="4cda3d47-1f0a-4ae4-868c-52c19b043f5d" ref="Microsoft.RedDog.Contract\Interface\PortfolioService:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Portfolio/PortfolioGroup/PortfolioService:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>