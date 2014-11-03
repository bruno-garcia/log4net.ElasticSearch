# -*- mode: ruby -*-
# vi: set ft=ruby :

# Vagrantfile API/syntax version. Don't touch unless you know what you're doing!
VAGRANTFILE_API_VERSION = "2"

Vagrant.configure(VAGRANTFILE_API_VERSION) do |config|
  config.vm.box = "ubuntu/trusty64"

  config.vm.box_url = "https://vagrantcloud.com/ubuntu/boxes/trusty64"
config.vm.provider "virtualbox" do |v|
    v.customize ["modifyvm", :id, "--cpus", "2"]
    v.customize ["modifyvm", :id, "--memory", "1024"]
  end
  
  config.vm.provision :shell, :path => "vagrant/dev-setup.sh"

end
