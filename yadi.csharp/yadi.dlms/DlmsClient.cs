/*
 * YADI (Yet Another DLMS Implementation)
 * Copyright (C) 2018 Paulo Faco (paulofaco@gmail.com)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
/*
* YADI.CSHARP
* Ported from Java to C# by Gabriel.Habib (enggabrielhabib@gmail.com) & Enzo Furlan (enzo.furlan@live.com)
*/

namespace yadi.dlms
{
	using Cosem = yadi.dlms.cosem.Cosem;
	using CosemParameters = yadi.dlms.cosem.CosemParameters;
	using LnDescriptor = yadi.dlms.cosem.LnDescriptor;
	using LinkLayer = yadi.dlms.linklayer.LinkLayer;
	using LinkLayerException = yadi.dlms.linklayer.LinkLayerException;
	using PhyLayer = yadi.dlms.phylayer.PhyLayer;
    using PhyLayerParser = yadi.dlms.phylayer.PhyLayerParser;
    using PhyLayerException = yadi.dlms.phylayer.PhyLayerException;

	public class DlmsClient
	{

		private Cosem cosem;
		private LinkLayer link;

		/// <summary>
		/// Creates a new Dlms instance, a facade to facilitate the usage of the Cosem, LinkLayer and PhyLayer. </summary>
		/// <param name="link"> link layer object </param>
		public DlmsClient(LinkLayer link) : this(link, new CosemParameters())
		{
		}

		/// <summary>
		/// Creates a new Dlms instance, a facade to facilitate the usage of the Cosem, LinkLayer and PhyLayer. </summary>
		/// <param name="link"> link layer object </param>
		/// <param name="params"> cosem parameters </param>
		public DlmsClient(LinkLayer link, CosemParameters parameters)
		{
			this.link = link;
			this.cosem = new Cosem(parameters);
		}

		/// <summary>
		/// Retrieves the parameters object </summary>
		/// <returns> the CosemParameters associated to this DlmsClient </returns>
		public CosemParameters getParameters()
		{
            return cosem.getParameters();
		}

		/// <summary>
		/// Connects to the server </summary>
		/// <param name="phy"> PhyLayer to transmit / receive bytes </param>
		/// <exception cref="PhyLayerException"> </exception>
		/// <exception cref="DlmsException"> </exception>
		/// <exception cref="LinkLayerException">  </exception>
		public void connect(PhyLayer phy)
		{
			cosem.reset();
			link.connect(phy);
			do
			{
				link.send(phy, cosem.connectionRequest());
			} while (!cosem.parseConnectionResponse(link.read(phy)));
		}


		/// 
		/// <param name="phy"> PhyLayer to transmit / receive bytes </param>
		/// <exception cref="PhyLayerException"> </exception>
		/// <exception cref="DlmsException"> </exception>
		/// <exception cref="LinkLayerException">  </exception>
		public void disconnect(PhyLayer phy)
		{
			link.disconnect(phy);
		}

		/// <summary>
		/// Performs a GET operation </summary>
		/// <param name="phy"> PhyLayer to transmit / receive bytes </param>
		/// <param name="obj"> Long-name descriptor of the objected to be accessed </param>
		/// <exception cref="PhyLayerException"> </exception>
		/// <exception cref="DlmsException"> </exception>
		/// <exception cref="LinkLayerException">  </exception>
		public void get(PhyLayer phy, LnDescriptor obj)
		{
			do
			{
				link.send(phy, cosem.requestGet(obj));
			} while (!cosem.parseGetResponse(obj, link.read(phy)));
		}

		/// <summary>
		/// Performs a SET operation </summary>
		/// <param name="phy"> PhyLayer to transmit / receive bytes </param>
		/// <param name="obj"> Long-name descriptor of the objected to be accessed </param>
		/// <exception cref="PhyLayerException"> </exception>
		/// <exception cref="DlmsException"> </exception>
		/// <exception cref="LinkLayerException">  </exception>
		public void set(PhyLayer phy, LnDescriptor obj)
		{
			do
			{
				link.send(phy, cosem.requestSet(obj));
			} while (!cosem.parseSetResponse(obj, link.read(phy)));
		}

		/// <summary>
		/// Performs a ACTION operation </summary>
		/// <param name="phy"> PhyLayer to transmit / receive bytes </param>
		/// <param name="obj"> Long-name descriptor of the objected to be accessed </param>
		/// <exception cref="PhyLayerException"> </exception>
		/// <exception cref="DlmsException"> </exception>
		/// <exception cref="LinkLayerException">  </exception>
		public void action(PhyLayer phy, LnDescriptor obj)
		{
			do
			{
				link.send(phy, cosem.requestAction(obj));
			} while (!cosem.parseActionResponse(obj, link.read(phy)));
		}

	}

}