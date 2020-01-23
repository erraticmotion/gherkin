# -*- mode: ruby -*-
# # vi: set ft=ruby :

# VM options set during `yo vagrant-dotnet`
$vm_box = "ubuntu-16.04-dotnet-sdk"
$num_instances = "1".to_i
$instance_name_prefix = "core"
$vm_memory = "1024".to_i
$vm_cpus = "2".to_i

require 'fileutils'

Vagrant.require_version ">= 1.6.2"

# Make sure the vagrant-ignition plugin is installed
required_plugins = %w(vagrant-ignition)

plugins_to_install = required_plugins.select { |plugin| not Vagrant.has_plugin? plugin }
if not plugins_to_install.empty?
  puts "Installing plugins: #{plugins_to_install.join(' ')}"
  if system "vagrant plugin install #{plugins_to_install.join(' ')}"
    exec "vagrant #{ARGV.join(' ')}"
  else
    abort "Installation of one or more plugins has failed. Aborting."
  end
end

# Enable port forwarding from guest(s) to host machine, syntax is: { 80 => 8080 }, auto correction is enabled by default.
$forwarded_ports = {}

# Enable port forwarding of Docker TCP socket
# Set to the TCP port you want exposed on the *host* machine, default is 2375
# If 2375 is used, Vagrant will auto-increment (e.g. in the case of $num_instances > 1)
# You can then use the docker tool locally by setting the following env var:
#   export DOCKER_HOST='tcp://127.0.0.1:2375'
#$expose_docker_tcp=2375

Vagrant.configure('2') do |config|

  config.vm.provider "vmware_workstation"
  config.vm.box = $vm_box
  
  if Vagrant.has_plugin?("vagrant-cachier")
    config.cache.scope = :box
  end

  (1..$num_instances).each do |i|
    config.vm.define vm_name = "%s-%02d" % [$instance_name_prefix, i] do |config|
      config.vm.hostname = vm_name

      if $expose_docker_tcp
        config.vm.network "forwarded_port", guest: 2375, host: ($expose_docker_tcp + i - 1), auto_correct: true
      end

      $forwarded_ports.each do |guest, host|
        config.vm.network "forwarded_port", guest: guest, host: host, auto_correct: true
      end

      #
      # VMware workstation settings
      #
      ["vmware_fusion", "vmware_workstation"].each do |vmware|
        config.vm.provider vmware do |v|
          v.gui = false
          v.vmx['memsize'] = $vm_memory
          v.vmx['numvcpus'] = $vm_cpus
          
          # Required for docker-machine
          v.vmx["vhv.enable"] = "TRUE"

          # Use paravirtualized virtual hardware on VMW hypervisors
          v.vmx['ethernet0.virtualDev'] = 'vmxnet3'
          v.vmx['scsi0.virtualDev'] = 'pvscsi'

          # persistent device naming whitelist
          v.whitelist_verified = true
        end
      end

      private_ip = "10.0.0.#{i+100}"
      config.vm.network "private_network", ip: private_ip
      config.vm.synced_folder ".", "/home/vagrant/src", create: true
    end
  end  
end
